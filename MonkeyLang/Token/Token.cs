using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

namespace MonkeyLang.Token
{
    public class Token
    {
        public string Type { get; set; }
        public string Literal { get; set; }

        private static Dictionary<string, string> keywords = new Dictionary<string, string>()
        {
            { "fn", TokenTypes.FUNCTION },
            { "let", TokenTypes.LET },
            { "true", TokenTypes.TRUE },
            { "false", TokenTypes.FALSE },
            { "if", TokenTypes.IF },
            { "else", TokenTypes.ELSE },
            { "return", TokenTypes.RETURN },
        };

        public static string LookupIdent(string ident)
        {
            string tok;
            if(keywords.TryGetValue(ident, out tok))
            {
                return tok;
            }
            return TokenTypes.IDENT;
        }
    }

    public static class TokenTypes
    {
        public const string ILLEGAL = "ILLEGAL";
        public const string EOF = "EOF";

        public const string IDENT = "IDENT";
        public const string INT = "INT";

        public const string ASSIGN = "=";
        public const string PLUS = "+";
        public const string MINUS = "-";
        public const string BANG = "!";
        public const string ASTERISK = "*";
        public const string SLASH = "/";

        public const string LT = "<";
        public const string GT = ">";

        public const string COMMA = ",";
        public const string SEMICOLON = ";";

        public const string LPAREN = "(";
        public const string RPAREN = ")";
        public const string LBRACE = "{";
        public const string RBRACE = "}";

        public const string FUNCTION = "FUNCTION";
        public const string LET = "LET";
        public const string TRUE = "TRUE";
        public const string FALSE = "FALSE";
        public const string IF = "IF";
        public const string ELSE = "ELSE";
        public const string RETURN = "RETURN";

        public const string EQ = "==";
        public const string NOT_EQ = "!=";
    }
}
