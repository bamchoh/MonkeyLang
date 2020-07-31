using Microsoft.VisualStudio.TestTools.UnitTesting;
using MonkeyLang;
using MonkeyLang.Token;
using MonkeyLang.Ast;
using MonkeyLang.Lexer;
using MonkeyLang.Parser;
using System.Collections.Generic;
using System.Threading;
using System;
using System.Net;
using System.Linq.Expressions;
using System.Linq;

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

        [DataRow(@"foobar;", "foobar")]
        [DataTestMethod]
        public void TestIdentifierExpression(string input, string value)
        {
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

            Assert.AreEqual(ident.Value, value, string.Format("ident.Value not {0}. got={1}", value, ident.Value));

            Assert.AreEqual(ident.TokenLiteral(), value, string.Format("ident.TokenLiteral not {0}. got={1}", value, ident.TokenLiteral()));
        }

        [DataRow(@"5;", 5, "5")]
        [DataTestMethod]
        public void TestIntegerLiteralExpression(string input, Int64 expVal, string expLit)
        {
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

            Assert.AreEqual(literal.Value, expVal, string.Format("literal.Value not {0}. got={1}", expVal, literal.Value));

            Assert.AreEqual(literal.TokenLiteral(), expLit, string.Format("literal.TokenLiteral not {0}. got={1}", expLit, literal.TokenLiteral()));
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

        [DataRow("5 + 5;", 5, "+", 5)]
        [DataRow("5 - 5;", 5, "-", 5)]
        [DataRow("5 * 5;", 5, "*", 5)]
        [DataRow("5 / 5;", 5, "/", 5)]
        [DataRow("5 > 5;", 5, ">", 5)]
        [DataRow("5 < 5;", 5, "<", 5)]
        [DataRow("5 == 5;", 5, "==", 5)]
        [DataRow("5 != 5;", 5, "!=", 5)]
        [DataRow("true == true", true, "==", true)]
        [DataRow("true != false", true, "!=", false)]
        [DataRow("false == false", false, "==", false)]
        [DataTestMethod]
        public void TestParsingInfixPrefixExpression(string input, object leftValue, string oper, object rightValue)
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

            if(!testInfixExpression(stmt.Expression, leftValue, oper, rightValue))
            {
                return;
            }
        }

        [DataRow("-a * b", "((-a) * b)")]
        [DataRow("!-a", "(!(-a))")]
        [DataRow("a + b + c", "((a + b) + c)")]
        [DataRow("a + b - c", "((a + b) - c)")]
        [DataRow("a * b * c", "((a * b) * c)")]
        [DataRow("a * b / c", "((a * b) / c)")]
        [DataRow("a + b / c", "(a + (b / c))")]
        [DataRow("a + b * c + d / e - f", "(((a + (b * c)) + (d / e)) - f)")]
        [DataRow("3 + 4; -5 * 5", "(3 + 4)((-5) * 5)")]
        [DataRow("5 > 4 == 3 < 4", "((5 > 4) == (3 < 4))")]
        [DataRow("5 < 4 != 3 > 4", "((5 < 4) != (3 > 4))")]
        [DataRow("3 + 4 * 5 == 3 * 1 + 4 * 5", "((3 + (4 * 5)) == ((3 * 1) + (4 * 5)))")]
        [DataRow("true", "true")]
        [DataRow("false", "false")]
        [DataRow("3 > 5 == false", "((3 > 5) == false)")]
        [DataRow("1 + (2 + 3) + 4", "((1 + (2 + 3)) + 4)")]
        [DataRow("(5 + 5) * 2", "((5 + 5) * 2)")]
        [DataRow("2 / (5 + 5)", "(2 / (5 + 5))")]
        [DataRow("-(5 + 5)", "(-(5 + 5))")]
        [DataRow("!(true == true)", "(!(true == true))")]
        [DataTestMethod]
        public void TestOperatorPrecedenceParsing(string input, string expected)
        {
            var l = new Lexer(input);
            var p = new Parser(l);
            var program = p.ParserProgram();
            checkParserErrors(p);

            var actual = program.String();
            Assert.AreEqual(actual, expected, string.Format("expected={0}, got={1}", expected, actual));
        }

        [TestMethod]
        public void TestIfExpression()
        {
            var input = @"if (x < y) { x }";

            var l = new Lexer(input);
            var p = new Parser(l);
            var program = p.ParserProgram();
            checkParserErrors(p);

            Assert.AreEqual(program.Statements.Count, 1, string.Format(
                "program.Statements does not contain {0} statements. got={1}", 1, program.Statements.Count));

            var stmt = (ExpressionStatement)program.Statements[0];

            Assert.IsNotNull(stmt, string.Format(
                "program.Statements[0] is not Ast.ExpressionStatement. got={0}", program.Statements[0].GetType()));

            var exp = (IfExpression)stmt.Expression;

            Assert.IsNotNull(exp, string.Format(
                "stmt.Expression is not Ast.IfExpression. got={0}", stmt.Expression.GetType()));

            if(!testInfixExpression(exp.Condition, "x", "<", "y"))
            {
                return;
            }

            Assert.AreEqual(exp.Consequence.Statements.Count, 1, string.Format(
                "consequence is not 1 statements. got={0}", exp.Consequence.Statements.Count));

            var consequence = (ExpressionStatement)exp.Consequence.Statements[0];

            Assert.IsNotNull(consequence, string.Format(
                "Statements[0] is not Ast.ExpressionStatement. got={0}", exp.Consequence.Statements[0]));

            if(!testIdentifier(consequence.Expression, "x"))
            {
                return;
            }

            Assert.IsNull(exp.Alternative, string.Format(
                "exp.Alternative.Statements was not null. got={0}", exp.Alternative));
        }

        [TestMethod]
        public void TestIfElseExpression()
        {
            var input = @"if (x < y) { x } else { y }";

            var l = new Lexer(input);
            var p = new Parser(l);
            var program = p.ParserProgram();
            checkParserErrors(p);

            Assert.AreEqual(program.Statements.Count, 1, string.Format(
                "program.Statements does not contain {0} statements. got={1}", 1, program.Statements.Count));

            var stmt = (ExpressionStatement)program.Statements[0];

            Assert.IsNotNull(stmt, string.Format(
                "program.Statements[0] is not Ast.ExpressionStatement. got={0}", program.Statements[0].GetType()));

            var exp = (IfExpression)stmt.Expression;

            Assert.IsNotNull(exp, string.Format(
                "stmt.Expression is not Ast.IfExpression. got={0}", stmt.Expression.GetType()));

            if (!testInfixExpression(exp.Condition, "x", "<", "y"))
            {
                return;
            }

            Assert.AreEqual(exp.Consequence.Statements.Count, 1, string.Format(
                "consequence is not 1 statements. got={0}", exp.Consequence.Statements.Count));

            var consequence = (ExpressionStatement)exp.Consequence.Statements[0];

            Assert.IsNotNull(consequence, string.Format(
                "Statements[0] is not Ast.ExpressionStatement. got={0}", exp.Consequence.Statements[0]));

            if (!testIdentifier(consequence.Expression, "x"))
            {
                return;
            }

            Assert.AreEqual(exp.Alternative.Statements.Count, 1, string.Format(
                "alternative is not 1 statements. got={0}", exp.Alternative.Statements.Count));

            var alternative = (ExpressionStatement)exp.Alternative.Statements[0];

            Assert.IsNotNull(alternative, string.Format(
                "Statements[0] is not Ast.ExpressionStatement. got={0}", exp.Alternative.Statements[0]));

            if (!testIdentifier(alternative.Expression, "y"))
            {
                return;
            }

        }

        [TestMethod]
        public void TestFunctionLiteralParsing()
        {
            var input = @"fn(x, y) { x + y; }";

            var l = new Lexer(input);
            var p = new Parser(l);
            var program = p.ParserProgram();
            checkParserErrors(p);

            Assert.AreEqual(program.Statements.Count, 1, string.Format(
                "program.Statements does not contain {0} statements. got={1}", 1, program.Statements.Count));

            var stmt = (ExpressionStatement)program.Statements[0];

            Assert.IsNotNull(stmt, string.Format(
                "program.Statements[0] is not Ast.ExpressionStatement. got={0}", program.Statements[0].GetType()));

            var function = (FunctionLiteral)stmt.Expression;

            Assert.IsNotNull(function, string.Format(
                "stmt.Expression is not Ast.FunctionLiteral. got={0}", stmt.Expression.GetType()));

            Assert.AreEqual(2, function.Parameters.Count, string.Format(
                "function literal parameters wrong. want 2, got={0}", function.Parameters.Count));

            testLiteralExpression(function.Parameters[0], "x");
            testLiteralExpression(function.Parameters[1], "y");

            Assert.AreEqual(1, function.Body.Statements.Count, string.Format(
                "function.Body.Statements has not 1 statements, got={0}", function.Body.Statements.Count));


            var bodyStmt = (ExpressionStatement)function.Body.Statements[0];

            Assert.IsNotNull(bodyStmt, string.Format(
                "function body stmt is not Ast.ExpressionStatement. got={0}", function.Body.Statements[0]));

            testInfixExpression(bodyStmt.Expression, "x", "+", "y");
        }

        [DataTestMethod]
        [DataRow("fn() {};", "")]
        [DataRow("fn(x) {};", "x")]
        [DataRow("fn(x, y, z) {};", "x,y,z")]
        public void TestFunctionParameterParsing(string input, string expectedParamsString)
        {
            string[] expectedParams;
            if (string.IsNullOrEmpty(expectedParamsString))
            {
                expectedParams = new string[] { };
            }
            else
            {
                expectedParams = expectedParamsString.Split(',');
            }

            var l = new Lexer(input);
            var p = new Parser(l);
            var program = p.ParserProgram();
            checkParserErrors(p);

            var stmt = (ExpressionStatement)program.Statements[0];
            var function = (FunctionLiteral)stmt.Expression;

            Assert.AreEqual(function.Parameters.Count, expectedParams.Length, string.Format(
                "length parameters wrong. want {0}, got={1}", expectedParams.Length, function.Parameters.Count));

            for(int i=0;i<expectedParams.Length;i++)
            {
                var ident = expectedParams[i];
                testLiteralExpression(function.Parameters[i], ident);
            }
        }

        private bool testIdentifier(MonkeyLang.Ast.Expression exp, string value)
        {
            var ident = (Identifier)exp;

            Assert.IsNotNull(ident, string.Format(
                "exp not Ast.Identifier. got={0}", exp.GetType().ToString()));

            Assert.AreEqual(ident.Value, value, string.Format(
                "ident.Value not {0}. got={1}", value, ident.Value));

            Assert.AreEqual(ident.TokenLiteral(), value, string.Format(
                "ident.TokenLiteral not {0}. got={1}", value, ident.TokenLiteral()));

            return true;
        }

        private bool testInfixExpression(MonkeyLang.Ast.Expression exp, object left, string oper, object right)
        {
            var opExp = (InfixExpression)exp;

            Assert.IsNotNull(opExp, string.Format(
                "exp is not Ast.InfixExpression. got={0}({1})", exp, exp.GetType().ToString()));

            if(!testLiteralExpression(opExp.Left, left))
            {
                return false;
            }

            Assert.AreEqual(opExp.Operator, oper, string.Format(
                "exp.Operator is not '{0}'. got={1}", oper, opExp.Operator));

            if(!testLiteralExpression(opExp.Right, right))
            {
                return false;
            }

            return true;
        }

        private bool testLiteralExpression(MonkeyLang.Ast.Expression exp, object expected)
        {
            var v = expected.GetType();
            if(v == typeof(int))
                return testIntegerLiteral(exp, Convert.ToInt64(expected));

            if (v == typeof(Int64))
                return testIntegerLiteral(exp, Convert.ToInt64(expected));

            if (v == typeof(string))
                return testIdentifier(exp, (string)expected);

            if (v == typeof(bool))
                return testBooleanLiteral(exp, (bool)expected);

            Assert.Fail(string.Format("type of exp not handled. got={0}", exp));

            return false;
        }

        private bool testBooleanLiteral(MonkeyLang.Ast.Expression exp, bool value)
        {
            var bo = (MonkeyLang.Ast.Boolean)exp;

            Assert.IsNotNull(bo, string.Format(
                "exp not Ast.Boolean. got={0}", exp));

            Assert.AreEqual(bo.Value, value, string.Format(
                "bo.Value not {0}. got={1}", value, bo.Value));

            Assert.AreEqual(bo.TokenLiteral(), string.Format("{0}", value.ToString().ToLower()), string.Format(
                "bo.TokenLiteral not {0}. got={1}", value, bo.TokenLiteral()));

            return true;
        }

        private bool testIntegerLiteral(MonkeyLang.Ast.Expression il, Int64 value)
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
