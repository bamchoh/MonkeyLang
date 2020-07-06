using Microsoft.VisualStudio.TestTools.UnitTesting;
using MonkeyLang.Token;
using MonkeyLang.Ast;
using MonkeyLang.Lexer;
using MonkeyLang.Parser;
using System.Collections.Generic;
using System.Threading;
using System;

namespace MonkeyLangTest
{
    [TestClass]
    public class AstTest
    {
        public class ExpectedTestParameter
        {
            public string ExpectedIdentifier;

            public ExpectedTestParameter(string identifier)
            {
                ExpectedIdentifier = identifier;
            }
        }

        [TestMethod]
        public void TestString()
        {
            var program = new Program()
            {
                Statements = new List<Statement>()
                {
                    new LetStatement()
                    {
                        Token = new Token()
                        {
                            Type = TokenTypes.LET,
                            Literal = "let"
                        },
                        Name = new Identifier()
                        {
                            Token = new Token()
                            {
                                Type = TokenTypes.IDENT,
                                Literal = "myVar"
                            },
                            Value = "myVar"
                        },
                        Value = new Identifier()
                        {
                            Token = new Token()
                            {
                                Type = TokenTypes.IDENT,
                                Literal = "anotherVar"
                            },
                            Value = "anotherVar"
                        }
                    }
                }
            };

            Assert.AreEqual(program.String(), "let myVar = anotherVar;", string.Format(
                "program.String() wrong. got={0}", program.String()));
        }
    }
}
