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

namespace MonkeyLangTest
{
    [TestClass]
    public class EvaluatorTest
    {
        [DataRow("5", 5)]
        [DataRow("10", 10)]
        [DataTestMethod]
        public void TestEvalIntegerExpression(string input, Int64 expected)
        {
            var evaluated = TestEval(input);
            testIntegerObject(evaluated, expected);
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
            var result = (MonInt)obj;
            Assert.IsNotNull(result, string.Format(
                "object is not Integer. got={0} ({1})", obj.GetType(), obj));

            Assert.AreEqual(result.Value, expected, string.Format(
                "object has wrong value. got={0}, want={1}", result.Value, expected));

            return true;
        }
    }
}
