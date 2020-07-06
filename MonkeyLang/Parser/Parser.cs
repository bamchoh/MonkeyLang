﻿using MonkeyLang.Ast;
using MonkeyLang.Token;
using System;
using System.Collections.Generic;
using System.Text;

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

        public Parser(Lexer.Lexer l)
        {
            this.l = l;
            nextToken();
            nextToken();
            errors = new List<string>();
            prefixParseFns = new Dictionary<string, Func<Expression>>();
            registerPrefix(TokenTypes.IDENT, parseIdentifier);
            infixParseFns = new Dictionary<string, Func<Expression, Expression>>();
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

        private Ast.Expression parseExpression(PrecedenceType i)
        {
            var prefix = prefixParseFns[curToken.Type];
            if(prefix == null)
            {
                return null;
            }
            var lestExp = prefix();

            return lestExp;
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
    }
}