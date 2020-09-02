using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using MonkeyLang.Object;

namespace MonkeyLang.Evaluator
{
    public class Evaluator
    {
        public static readonly MonNull NULL = new Object.MonNull();
        public static readonly MonBool TRUE = new Object.MonBool() { Value = true };
        public static readonly MonBool FALSE = new Object.MonBool() { Value = false };

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

            if(node is Ast.Boolean)
            {
                return nativeBoolToBooleanObject(((Ast.Boolean)node).Value);
            }

            if(node is Ast.PrefixExpression)
            {
                var _node = (Ast.PrefixExpression)node;
                var right = Eval(_node.Right);
                return evalPrefixExpression(_node.Operator, right);
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

        private static Object.MonBool nativeBoolToBooleanObject(bool input)
        {
            return input ? TRUE : FALSE;
        }

        private static Object.MonObj evalPrefixExpression(string oper, Object.MonObj right)
        {
            switch(oper)
            {
                case "!":
                    return evalBangOperatorExpression(right);
                case "-":
                    return evalMinusPrefixOperatorExpression(right);
                default:
                    return NULL;
            }
        }

        private static Object.MonObj evalBangOperatorExpression(Object.MonObj right)
        {
            if(right == TRUE)
            {
                return FALSE;
            }

            if(right == FALSE)
            {
                return TRUE;
            }

            if(right == NULL)
            {
                return TRUE;
            }

            return FALSE;
        }

        private static Object.MonObj evalMinusPrefixOperatorExpression(Object.MonObj right)
        {
            if(right.Type() != ObjectType.INTEGER_OBJ)
            {
                return NULL;
            }

            var value = ((Object.MonInt)right).Value;
            return new Object.MonInt { Value = -value };
        }
    }
}
