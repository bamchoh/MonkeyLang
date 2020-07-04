using System;
using System.Collections.Generic;
using System.Text;
using MonkeyLang.Token;

namespace MonkeyLang.Lexer
{
    public class Lexer
    {
        string input;
        int position;
        int readPosition;
        char ch;

        public Lexer(string input)
        {
            this.input = input;
            readChar();
        }

        public Token.Token NextToken()
        {
            var tok = new Token.Token();

            skipWhitespace();

            switch(ch) {
                case '=':
                    if(peekChar() == '=')
                    {
                        var c = ch;
                        readChar();
                        var literal = c.ToString() + ch.ToString();
                        tok = new Token.Token()
                        {
                            Type = TokenTypes.EQ,
                            Literal = literal,
                        };
                    }
                    else
                    {
                        tok = newToken(TokenTypes.ASSIGN, ch);
                    }
                    break;
                case '+':
                    tok = newToken(TokenTypes.PLUS, ch);
                    break;
                case '-':
                    tok = newToken(TokenTypes.MINUS, ch);
                    break;
                case '!':
                    if(peekChar() == '=')
                    {
                        var c = ch;
                        readChar();
                        var literal = c.ToString() + ch.ToString();
                        tok = new Token.Token()
                        {
                            Type = TokenTypes.NOT_EQ,
                            Literal = literal,
                        };
                    }
                    else
                    {
                        tok = newToken(TokenTypes.BANG, ch);
                    }
                    break;
                case '/':
                    tok = newToken(TokenTypes.SLASH, ch);
                    break;
                case '*':
                    tok = newToken(TokenTypes.ASTERISK, ch);
                    break;
                case '<':
                    tok = newToken(TokenTypes.LT, ch);
                    break;
                case '>':
                    tok = newToken(TokenTypes.GT, ch);
                    break;
                case ';':
                    tok = newToken(TokenTypes.SEMICOLON, ch);
                    break;
                case '(':
                    tok = newToken(TokenTypes.LPAREN, ch);
                    break;
                case ')':
                    tok = newToken(TokenTypes.RPAREN, ch);
                    break;
                case ',':
                    tok = newToken(TokenTypes.COMMA, ch);
                    break;
                case '{':
                    tok = newToken(TokenTypes.LBRACE, ch);
                    break;
                case '}':
                    tok = newToken(TokenTypes.RBRACE, ch);
                    break;
                case '\0':
                    tok.Literal = "";
                    tok.Type = TokenTypes.EOF;
                    break;
                default:
                    if(isLetter(ch))
                    {
                        tok.Literal = readIdentifier();
                        tok.Type = Token.Token.LookupIdent(tok.Literal);
                        return tok;
                    }
                    else if(isDigit(ch))
                    {
                        tok.Type = TokenTypes.INT;
                        tok.Literal = readNumber();
                        return tok;
                    }
                    else
                    {
                        tok = newToken(TokenTypes.ILLEGAL, ch);
                    }
                    break;
            }

            readChar();
            return tok;
        }

        private string readNumber()
        {
            var pos = position;
            while(isDigit(ch))
            {
                readChar();
            }
            return input.Substring(pos, position - pos);
        }

        private bool isDigit(char c)
        {
            return '0' <= c && c <= '9';
        }

        private void skipWhitespace()
        {
            while(ch == ' ' || ch == '\t' || ch == '\n' || ch == '\r')
            {
                readChar();
            }
        }

        private string readIdentifier()
        {
            var pos = position;
            while(isLetter(ch))
            {
                readChar();
            }
            return input.Substring(pos, position - pos);
        }

        private bool isLetter(char c)
        {
            return 'a' <= c && c <= 'z' || 'A' <= c && c <= 'Z' || ch == '_';
        }

        private Token.Token newToken(string t, char c)
        {
            return new Token.Token()
            {
                Type = t,
                Literal = c.ToString(),
            };
        }

        private void readChar()
        {
            if(readPosition >= input.Length)
            {
                ch = '\0';
            }
            else
            {
                ch = input[readPosition];
            }
            position = readPosition;
            readPosition++;
        }

        private char peekChar()
        {
            if(readPosition >= input.Length)
            {
                return '\0';
            }
            else
            {
                return input[readPosition];
            }
        }
    }
}
