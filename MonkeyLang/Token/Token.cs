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
        public static string ILLEGAL = "ILLEGAL";
        public static string EOF = "EOF";

        public static string IDENT = "IDENT";
        public static string INT = "INT";

        public static string ASSIGN = "=";
        public static string PLUS = "+";
        public static string MINUS = "-";
        public static string BANG = "!";
        public static string ASTERISK = "*";
        public static string SLASH = "/";

        public static string LT = "<";
        public static string GT = ">";

        public static string COMMA = ",";
        public static string SEMICOLON = ";";

        public static string LPAREN = "(";
        public static string RPAREN = ")";
        public static string LBRACE = "{";
        public static string RBRACE = "}";

        public static string FUNCTION = "FUNCTION";
        public static string LET = "LET";
        public static string TRUE = "TRUE";
        public static string FALSE = "FALSE";
        public static string IF = "IF";
        public static string ELSE = "ELSE";
        public static string RETURN = "RETURN";

        public static string EQ = "==";
        public static string NOT_EQ = "!=";
    }
}
