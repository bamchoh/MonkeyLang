using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;

namespace MonkeyLang.Ast
{
    public interface Node
    {
        public string TokenLiteral();
        public string String();
    }

    public interface Statement : Node
    {
    }

    public interface Expression : Node
    {
    }

    public class Program : Node
    {
        public List<Statement> Statements = new List<Statement>();

        public string TokenLiteral()
        {
            if(Statements.Count > 0)
            {
                return Statements[0].TokenLiteral();
            }
            else
            {
                return "";
            }
        }

        public string String()
        {
            var buf = new Utilities.StringBuffer();

            foreach(var s in Statements)
            {
                buf.Write(s.String());
            }

            var ss = buf.String();

            return ss;
        }
    }

    public class LetStatement : Statement
    {
        public Token.Token Token;
        public Identifier Name;
        public Expression Value;

        public string TokenLiteral()
        {
            return Token.Literal;
        }

        public string String()
        {
            var buf = new Utilities.StringBuffer();

            buf.Write(Token.Literal + " ");
            buf.Write(Name.String());
            buf.Write(" = ");

            if(Value != null)
            {
                buf.Write(Value.String());
            }

            buf.Write(";");

            return buf.String();
        }
    }

    public class Identifier : Expression
    {
        public Token.Token Token;
        public string Value;

        public string TokenLiteral()
        {
            return Token.Literal;
        }

        public string String()
        {
            return Value;
        }
    }

    public class ReturnStatement : Statement
    {
        public Token.Token Token;
        public Expression ReturnValue;

        public string TokenLiteral()
        {
            return Token.Literal;
        }

        public string String()
        {
            var buf = new Utilities.StringBuffer();

            buf.Write(TokenLiteral() + " ");

            if(ReturnValue != null)
            {
                buf.Write(ReturnValue.String());
            }

            buf.Write(";");

            return buf.String();
        }
    }

    public class ExpressionStatement : Statement
    {
        public Token.Token Token;
        public Expression Expression;

        public string TokenLiteral()
        {
            return Token.Literal;
        }

        public string String()
        {
            if(Expression != null)
            {
                return Expression.String();
            }

            return "";
        }
    }

    public class IntegerLiteral : Expression
    {
        public Token.Token Token;
        public Int64 Value;

        public string TokenLiteral()
        {
            return Token.Literal;
        }

        public string String()
        {
            return Token.Literal;
        }
    }

    public class PrefixExpression : Expression
    {
        public Token.Token Token;
        public string Operator;
        public Expression Right;

        public string TokenLiteral()
        {
            return Token.Literal;
        }

        public string String()
        {
            var buf = new Utilities.StringBuffer();

            buf.Write("(");
            buf.Write(Operator);
            buf.Write(Right.String());
            buf.Write(")");

            return buf.String();
        }
    }

    public class InfixExpression : Expression
    {
        public Token.Token Token;
        public Expression Left;
        public string Operator;
        public Expression Right;

        public string TokenLiteral()
        {
            return Token.Literal;
        }

        public string String()
        {
            var buf = new Utilities.StringBuffer();

            buf.Write("(");
            buf.Write(Left.String());
            buf.Write(" " + Operator + " ");
            buf.Write(Right.String());
            buf.Write(")");

            return buf.String(); ;
        }
    }
}
