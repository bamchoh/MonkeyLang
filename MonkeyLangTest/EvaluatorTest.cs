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
using MonkeyLang.Object;
using MonkeyLang.Evaluator;
using System.ComponentModel;

namespace MonkeyLangTest
{
    [TestClass]
    public class EvaluatorTest
    {
        [DataRow("5", 5)]
        [DataRow("10", 10)]
        [DataRow("-5", -5)]
        [DataRow("-10", -10)]
        [DataTestMethod]
        public void TestEvalIntegerExpression(string input, Int64 expected)
        {
            var evaluated = TestEval(input);
            testIntegerObject(evaluated, expected);
        }

        [DataRow("true", true)]
        [DataRow("false", false)]
        [DataTestMethod]
        public void TestEvalBooleanExpression(string input, bool expected)
        {
            var evaluated = TestEval(input);
            testBooleanObject(evaluated, expected);
        }

        [DataRow("!true", false)]
        [DataRow("!false", true)]
        [DataRow("!5", false)]
        [DataRow("!!true", true)]
        [DataRow("!!false", false)]
        [DataRow("!!5", true)]
        [DataTestMethod]
        public void TestBangOperator(string input, bool expected)
        {
            var evaluated = TestEval(input);
            testBooleanObject(evaluated, expected);
        }

        private MonObj TestEval(string input)
        {
            var l = new Lexer(input);
            var p = new Parser(l);
            var program = p.ParserProgram();

            return Evaluator.Eval(program);
        }

        private bool testIntegerObject(MonObj obj, Int64 expected)
        {
            Assert.IsNotNull(obj, "object is null");

            var result = (MonInt)obj;
            Assert.IsNotNull(result, string.Format(
                "object is not Integer. got={0} ({1})", obj.GetType(), obj));

            Assert.AreEqual(result.Value, expected, string.Format(
                "object has wrong value. got={0}, want={1}", result.Value, expected));

            return true;
        }

        private bool testBooleanObject(MonObj obj, bool expected)
        {
            Assert.IsNotNull(obj, "object is null");

            var result = (MonBool)obj;
            Assert.IsNotNull(result, string.Format(
                "object is not Boolean. got={0} ({1})", obj.GetType(), obj));

            Assert.AreEqual(result.Value, expected, string.Format(
                "object has wrong value. got={0}, want={1}", result.Value, expected));

            return true;
        }
    }
}
