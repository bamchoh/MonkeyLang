using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using MonkeyLang.Object;

namespace MonkeyLang.Evaluator
{
    public class Evaluator
    {
        public static MonObj Eval(Ast.Node node)
        {
            if(node is Ast.Program)
            {
                return evalStatements(((Ast.Program)node).Statements);
            }

            if(node is Ast.ExpressionStatement)
            {
                return Eval(((Ast.ExpressionStatement)node).Expression);
            }

            if(node is Ast.IntegerLiteral)
            {
                return new MonInt() { Value = ((Ast.IntegerLiteral)node).Value };
            }

            return null;
        }

        private static MonObj evalStatements(List<Ast.Statement> stmts)
        {
            MonObj result = null;

            foreach(var statement in stmts)
            {
                result = Eval(statement);
            }

            return result;
        }
    }
}
