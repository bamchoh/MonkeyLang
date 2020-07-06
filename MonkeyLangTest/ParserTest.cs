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
    public class ParserTest
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
        public void TestLetStatements()
        {
            string input = @"
let x = 5;
let y = 10;
let foobar = 838383;
";
            var l = new Lexer(input);
            var p = new Parser(l);

            var program = p.ParserProgram();

            checkParserErrors(p);

            Assert.IsNotNull(program, "ParseProgram() returned nil");

            Assert.AreEqual(program.Statements.Count, 3,
                string.Format("program.Statements does not contain 3 statements. got={0}", program.Statements.Count));

            var tests = new List<ExpectedTestParameter>()
            {
                new ExpectedTestParameter("x"),
                new ExpectedTestParameter("y"),
                new ExpectedTestParameter("foobar"),
            };

            for(int i = 0;i<tests.Count;i++)
            {
                var stmt = program.Statements[i];
                testLetStatement(stmt, tests[i].ExpectedIdentifier);
            }
        }

        [TestMethod]
        public void TestReturnStatements()
        {
            string input = @"
return 5;
return 10;
return 993322;
";
            var l = new Lexer(input);
            var p = new Parser(l);

            var program = p.ParserProgram();
            checkParserErrors(p);

            Assert.AreEqual(program.Statements.Count, 3,
                string.Format("program.Statements does not contain 3 statements. got={0}", program.Statements.Count));

            foreach(var stmt in program.Statements)
            {
                var returnStmt = (ReturnStatement)stmt;
                Assert.IsNotNull(returnStmt, string.Format("stmt not Ast.ReturnStatement. got={0}", stmt));
                Assert.AreEqual(returnStmt.TokenLiteral(), "return", string.Format("returnStmt.TokenLiteral not 'return', got {0}",
                    returnStmt.TokenLiteral()));
            }
        }

        [TestMethod]
        public void TestIdentifierExpression()
        {
            string input = @"foobar;";

            var l = new Lexer(input);
            var p = new Parser(l);
            var program = p.ParserProgram();
            checkParserErrors(p);
            Assert.AreEqual(program.Statements.Count, 1,
                string.Format("program has not enough statements. got={0}", program.Statements.Count));

            var stmt = (ExpressionStatement)program.Statements[0];
            Assert.IsNotNull(stmt, string.Format("program.Statements[0] is not Ast.ExpressionStatement. got={0}", program.Statements[0]));

            var ident = (Identifier)stmt.Expression;
            Assert.IsNotNull(ident, string.Format("exp not Ast.Identifier. got={0}", stmt.Expression));

            Assert.AreEqual(ident.Value, "foobar", string.Format("ident.Value not {0}. got={1}", "foobar", ident.Value));

            Assert.AreEqual(ident.TokenLiteral(), "foobar", string.Format("ident.TokenLiteral not {0}. got={1}", "foobar", ident.TokenLiteral()));
        }

        private void testLetStatement(Statement s, string name)
        {
            Assert.AreEqual(s.TokenLiteral(), "let", string.Format("s.TokenLiteral not 'let'. got={0}", s.TokenLiteral()));

            var letStmt = (LetStatement)s;
            Assert.IsNotNull(letStmt, string.Format("s not Ast.LetStatement. got={0}", s));
            Assert.AreEqual(letStmt.Name.Value, name, string.Format("letStmt.Name.Value not '{0}'. got={1}", name, letStmt.Name.Value));
            Assert.AreEqual(letStmt.Name.TokenLiteral(), name, string.Format("letStmt.Name.TokenLiteral() not '{0}'. got={1}", name, letStmt.Name.TokenLiteral()));
        }

        private void checkParserErrors(Parser p)
        {
            var errors = p.Errors();
            Assert.AreEqual(errors.Count, 0, new Func<string>(() =>
            {
                var messages = new List<string>();
                messages.Add(string.Format("parser has {0} errors", errors.Count));
                foreach(var msg in errors)
                {
                    messages.Add(string.Format("parser error: {0}", msg));
                }
                return string.Join("\n", messages);
            })());
        }
    }
}
