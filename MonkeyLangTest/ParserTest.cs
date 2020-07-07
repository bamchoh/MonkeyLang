using Microsoft.VisualStudio.TestTools.UnitTesting;
using MonkeyLang.Token;
using MonkeyLang.Ast;
using MonkeyLang.Lexer;
using MonkeyLang.Parser;
using System.Collections.Generic;
using System.Threading;
using System;
using System.Net;

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

        [TestMethod]
        public void TestIntegerLiteralExpression()
        {
            var input = @"5;";

            var l = new Lexer(input);
            var p = new Parser(l);
            var program = p.ParserProgram();
            checkParserErrors(p);

            Assert.AreEqual(program.Statements.Count, 1,
                string.Format("program has not enough statements. got={0}", program.Statements.Count));

            var stmt = (ExpressionStatement)program.Statements[0];

            Assert.IsNotNull(stmt, string.Format("program.Statements[0] is not Ast.ExpressionStatement. got={0}",
                program.Statements[0].GetType().ToString()));

            var literal = (IntegerLiteral)stmt.Expression;

            Assert.IsNotNull(literal, string.Format("exp not Ast.IntegerLiteral. got={0}", stmt.Expression));

            Assert.AreEqual(literal.Value, 5, string.Format("literal.Value not {0}. got={1}", 5, literal.Value));

            Assert.AreEqual(literal.TokenLiteral(), "5", string.Format("literal.TokenLiteral not {0}. got={1}", "5", literal.TokenLiteral()));
        }

        public class prefixExpressionTestExpectedParams
        {
            public string Input { get; set; }
            public string Operator { get; set; }
            public Int64 IntegerValue { get; set; }
        }

        [DataRow("!5", "!", 5)]
        [DataRow("-15", "-", 15)]
        [DataTestMethod]
        public void TestParsingPrefixExpression(string input, string oper, Int64 integerValue)
        {
            var l = new Lexer(input);
            var p = new Parser(l);
            var program = p.ParserProgram();
            checkParserErrors(p);

            Assert.AreEqual(program.Statements.Count, 1,
                string.Format("program.Statements does not contain {0} statements. got={1}",
                1, program.Statements.Count));

            var stmt = (ExpressionStatement)program.Statements[0];

            Assert.IsNotNull(stmt, string.Format("program.Statements[0] is not Ast.ExpressionStatement. got={0}",
                program.Statements[0]));

            var exp = (PrefixExpression)stmt.Expression;

            Assert.IsNotNull(exp, string.Format("stmt is not Ast.PrefixExpression. got={0}", stmt.Expression));

            Assert.AreEqual(exp.Operator, oper, string.Format("exp.Operator is not '{0}'. got={1}", oper, exp.Operator));

            if(!testIntegerLiteral(exp.Right, integerValue))
            {
                return;
            }
        }

        private bool testIntegerLiteral(Expression il, Int64 value)
        {
            var integ = (IntegerLiteral)il;

            Assert.IsNotNull(integ, string.Format("il not Ast.IntegerLiteral. got={0}", il));

            Assert.AreEqual(integ.Value, value, string.Format("integ.Value not {0}. got={1}", value, integ.Value));

            Assert.AreEqual(integ.TokenLiteral(), string.Format("{0}", value),
                string.Format("integ.TokenLiteral not {0}. got={1}", value, integ.TokenLiteral()));

            return true;
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
