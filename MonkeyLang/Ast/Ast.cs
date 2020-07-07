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
            var ms = new MemoryStream();

            foreach(var s in Statements)
            {
                ms.Write(Encoding.UTF8.GetBytes(s.String()));
            }

            return Encoding.UTF8.GetString(ms.ToArray());
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
            var ms = new MemoryStream();

            ms.Write(Encoding.UTF8.GetBytes(Token.Literal + " "));
            ms.Write(Encoding.UTF8.GetBytes(Name.String()));
            ms.Write(Encoding.UTF8.GetBytes(" = "));

            if(Value != null)
            {
                ms.Write(Encoding.UTF8.GetBytes(Value.String()));
            }

            ms.Write(Encoding.UTF8.GetBytes(";"));

            return Encoding.UTF8.GetString(ms.ToArray());
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
            var ms = new MemoryStream();

            ms.Write(Encoding.UTF8.GetBytes(TokenLiteral() + " "));

            if(ReturnValue != null)
            {
                ms.Write(Encoding.UTF8.GetBytes(ReturnValue.String()));
            }

            ms.Write(Encoding.UTF8.GetBytes(";"));

            return Encoding.UTF8.GetString(ms.ToArray());
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
            var ms = new MemoryStream();

            ms.Write(Encoding.UTF8.GetBytes("("));
            ms.Write(Encoding.UTF8.GetBytes(Operator));
            ms.Write(Encoding.UTF8.GetBytes(Right.String()));
            ms.Write(Encoding.UTF8.GetBytes(")"));

            return Encoding.UTF8.GetString(ms.ToArray());
        }
    }
}
