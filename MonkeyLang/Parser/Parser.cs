﻿using MonkeyLang.Ast;
using MonkeyLang.Token;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Channels;

namespace MonkeyLang.Parser
{
    public class Parser
    {
        Lexer.Lexer l;
        Token.Token curToken;
        Token.Token peekToken;
        List<string> errors;
        Dictionary<string, Func<Expression>> prefixParseFns;
        Dictionary<string, Func<Expression, Expression>> infixParseFns;

        enum PrecedenceType
        {
            LOWEST = 1,
            EQUALS,
            LESSGREATER,
            SUM,
            PRODUCT,
            PREFIX,
            CALL,
        }

        Dictionary<string, PrecedenceType> precedences = new Dictionary<string, PrecedenceType>()
        {
            { TokenTypes.EQ, PrecedenceType.EQUALS },
            { TokenTypes.NOT_EQ, PrecedenceType.EQUALS },
            { TokenTypes.LT, PrecedenceType.LESSGREATER },
            { TokenTypes.GT, PrecedenceType.LESSGREATER },
            { TokenTypes.PLUS, PrecedenceType.SUM },
            { TokenTypes.MINUS, PrecedenceType.SUM },
            { TokenTypes.SLASH, PrecedenceType.PRODUCT },
            { TokenTypes.ASTERISK, PrecedenceType.PRODUCT },
        };

        public Parser(Lexer.Lexer l)
        {
            this.l = l;
            nextToken();
            nextToken();
            errors = new List<string>();

            prefixParseFns = new Dictionary<string, Func<Expression>>();
            registerPrefix(TokenTypes.IDENT, parseIdentifier);
            registerPrefix(TokenTypes.INT, parseIntegerLiteral);
            registerPrefix(TokenTypes.BANG, parsePrefixExpression);
            registerPrefix(TokenTypes.MINUS, parsePrefixExpression);

            infixParseFns = new Dictionary<string, Func<Expression, Expression>>();
            registerInfix(TokenTypes.PLUS, parseInfixExpression);
            registerInfix(TokenTypes.MINUS, parseInfixExpression);
            registerInfix(TokenTypes.SLASH, parseInfixExpression);
            registerInfix(TokenTypes.ASTERISK, parseInfixExpression);
            registerInfix(TokenTypes.EQ, parseInfixExpression);
            registerInfix(TokenTypes.NOT_EQ, parseInfixExpression);
            registerInfix(TokenTypes.LT, parseInfixExpression);
            registerInfix(TokenTypes.GT, parseInfixExpression);
        }

        public List<string> Errors()
        {
            return errors;
        }

        private void peekError(string t)
        {
            var msg = string.Format("expected next token to be {0}, got {1} instead", t, peekToken.Type);
            errors.Add(msg);
        }

        private PrecedenceType peekPrecedence()
        {
            PrecedenceType p;
            var ok = precedences.TryGetValue(peekToken.Type, out p);
            if (ok)
                return p;

            return PrecedenceType.LOWEST;
        }

        private PrecedenceType curPrecedence()
        {
            PrecedenceType p;
            var ok = precedences.TryGetValue(curToken.Type, out p);
            if (ok)
                return p;

            return PrecedenceType.LOWEST;
        }

        public Ast.Program ParserProgram()
        {
            var program = new Program();
            while(curToken.Type != TokenTypes.EOF)
            {
                var stmt = parseStatement();
                if(stmt != null)
                {
                    program.Statements.Add(stmt);
                }
                nextToken();
            }
            return program;
        }

        private void nextToken()
        {
            curToken = peekToken;
            peekToken = l.NextToken();
        }

        private Ast.Statement parseStatement()
        {
            switch(curToken.Type)
            {
                case TokenTypes.LET:
                    return parseLetStatement();
                case TokenTypes.RETURN:
                    return parseReturnStatement();
                default:
                    return parseExpressionStatement();
            }
        }

        private Ast.LetStatement parseLetStatement()
        {
            var stmt = new Ast.LetStatement() { Token = curToken };

            if(!expectPeek(TokenTypes.IDENT))
            {
                return null;
            }

            stmt.Name = new Ast.Identifier() { Token = curToken, Value = curToken.Literal };

            if(!expectPeek(TokenTypes.ASSIGN))
            {
                return null;
            }

            while(!curTokenIs(TokenTypes.SEMICOLON))
            {
                nextToken();
            }

            return stmt;
        }

        private Ast.ReturnStatement parseReturnStatement()
        {
            var stmt = new Ast.ReturnStatement() { Token = curToken };

            nextToken();

            while(!curTokenIs(TokenTypes.SEMICOLON))
            {
                nextToken();
            }

            return stmt;
        }

        private Ast.ExpressionStatement parseExpressionStatement()
        {
            var stmt = new ExpressionStatement() { Token = curToken };

            stmt.Expression = parseExpression(PrecedenceType.LOWEST);

            if(peekTokenIs(TokenTypes.SEMICOLON))
            {
                nextToken();
            }

            return stmt;
        }

        private Ast.Expression parseExpression(PrecedenceType precedence)
        {
            Func<Expression> prefix;
            var ok = prefixParseFns.TryGetValue(curToken.Type, out prefix);
            if(!ok)
            {
                noPrefixParseFnError(curToken.Type);
                return null;
            }
            var leftExp = prefix();

            while(!peekTokenIs(TokenTypes.SEMICOLON) && precedence < peekPrecedence())
            {
                Func<Expression, Expression> infix;
                ok = infixParseFns.TryGetValue(peekToken.Type, out infix);
                if(!ok)
                {
                    return leftExp;
                }

                nextToken();

                leftExp = infix(leftExp);
            }

            return leftExp;
        }

        private Ast.Expression parseIntegerLiteral()
        {
            var lit = new Ast.IntegerLiteral() { Token = curToken };
            Int64 value;
            var err = Int64.TryParse(curToken.Literal, out value);
            if(!err)
            {
                var msg = string.Format("could not parse {0} as integer", curToken.Literal);
                errors.Add(msg);
                return null;
            }

            lit.Value = value;

            return lit;
        }

        private Ast.Expression parsePrefixExpression()
        {
            var expression = new PrefixExpression()
            {
                Token = curToken,
                Operator = curToken.Literal,
            };

            nextToken();

            expression.Right = parseExpression(PrecedenceType.PREFIX);

            return expression;
        }

        private bool curTokenIs(string t)
        {
            return curToken.Type == t;
        }

        private bool peekTokenIs(string t)
        {
            return peekToken.Type == t;
        }

        private bool expectPeek(string t)
        {
            if(peekTokenIs(t))
            {
                nextToken();
                return true;
            }
            else
            {
                peekError(t);
                return false;
            }
        }

        private void registerPrefix(string tokenType, Func<Expression> fn)
        {
            prefixParseFns[tokenType] = fn;
        }

        private void registerInfix(string tokenType, Func<Expression, Expression> fn)
        {
            infixParseFns[tokenType] = fn;
        }

        private Expression parseIdentifier()
        {
            return new Identifier()
            {
                Token = curToken,
                Value = curToken.Literal,
            };
        }

        private void noPrefixParseFnError(string t)
        {
            var msg = string.Format("no prefix parse function for {0} found", t);
            errors.Add(msg);
        }

        private Ast.Expression parseInfixExpression(Ast.Expression left)
        {
            var expression = new Ast.InfixExpression()
            {
                Token = curToken,
                Operator = curToken.Literal,
                Left = left,
            };

            var precedence = curPrecedence();
            nextToken();
            expression.Right = parseExpression(precedence);

            return expression;
        }
    }
}
