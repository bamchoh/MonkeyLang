using Microsoft.VisualStudio.TestTools.UnitTesting;
using MonkeyLang.Token;
using MonkeyLang.Lexer;
using System.Collections.Generic;

namespace MonkeyLangTest
{
    [TestClass]
    public class LexerTest
    {
        public class ep
        {
            public string ExpectedType { get; }
            public string ExpectedLiteral { get; }

            public ep(string t, string l)
            {
                ExpectedType = t;
                ExpectedLiteral = l;
            }
        }

        [TestMethod]
        public void TestNextToken()
        {
            var input = @"let five = 5;
let ten = 10;

let add = fn(x, y) {
  x + y;
};

let result = add(five, ten);
";

            var tests = new List<ep>()
            {
                new ep(TokenTypes.LET, "let"),
                new ep(TokenTypes.IDENT, "five"),
                new ep(TokenTypes.ASSIGN, "="),
                new ep(TokenTypes.INT, "5"),
                new ep(TokenTypes.SEMICOLON, ";"),
                new ep(TokenTypes.LET, "let"),
                new ep(TokenTypes.IDENT, "ten"),
                new ep(TokenTypes.ASSIGN, "="),
                new ep(TokenTypes.INT, "10"),
                new ep(TokenTypes.SEMICOLON, ";"),
                new ep(TokenTypes.LET, "let"),
                new ep(TokenTypes.IDENT, "add"),
                new ep(TokenTypes.ASSIGN, "="),
                new ep(TokenTypes.FUNCTION, "fn"),
                new ep(TokenTypes.LPAREN, "("),
                new ep(TokenTypes.IDENT, "x"),
                new ep(TokenTypes.COMMA, ","),
                new ep(TokenTypes.IDENT, "y"),
                new ep(TokenTypes.RPAREN, ")"),
                new ep(TokenTypes.LBRACE, "{"),
                new ep(TokenTypes.IDENT, "x"),
                new ep(TokenTypes.PLUS, "+"),
                new ep(TokenTypes.IDENT, "y"),
                new ep(TokenTypes.SEMICOLON, ";"),
                new ep(TokenTypes.RBRACE, "}"),
                new ep(TokenTypes.SEMICOLON, ";"),
                new ep(TokenTypes.LET, "let"),
                new ep(TokenTypes.IDENT, "result"),
                new ep(TokenTypes.ASSIGN, "="),
                new ep(TokenTypes.IDENT, "add"),
                new ep(TokenTypes.LPAREN, "("),
                new ep(TokenTypes.IDENT, "five"),
                new ep(TokenTypes.COMMA, ","),
                new ep(TokenTypes.IDENT, "ten"),
                new ep(TokenTypes.RPAREN, ")"),
                new ep(TokenTypes.SEMICOLON, ";"),
                new ep(TokenTypes.EOF, ""),
            };

            var l = new Lexer(input);

            for(int i = 0;i < tests.Count; i++)
            {
                Token tok = l.NextToken();

                Assert.AreEqual(tests[i].ExpectedType, tok.Type, string.Format("tests[{0}] - tokentype wrong. expected={1}, got={2}",
                    i, tests[i].ExpectedType, tok.Type));

                Assert.AreEqual(tests[i].ExpectedLiteral, tok.Literal, string.Format("tests[{0}] - literal wrong. expected={1}, got={2}",
                    i, tests[i].ExpectedLiteral, tok.Literal));
            }
        }
    }
}
