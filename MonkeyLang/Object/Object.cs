using System;
using System.Collections.Generic;
using System.Text;

namespace MonkeyLang.Object
{
    public class ObjectType
    {
        public const string INTEGER_OBJ = "INTEGER";
        public const string BOOLEAN_OBJ = "BOOLEAN";
        public const string NULL_OBJ = "NULL";
    }

    public interface MonObj
    {
        public string Type();
        public string Inspect();
    }

    public class MonInt : MonObj
    {
        public Int64 Value;

        public string Type()
        {
            return ObjectType.INTEGER_OBJ;
        }

        public string Inspect()
        {
            return string.Format("{0}", Value);
        }
    }

    public class MonBool : MonObj
    {
        public bool Value;

        public string Type()
        {
            return ObjectType.BOOLEAN_OBJ;
        }

        public string Inspect()
        {
            return string.Format("{0}", Value);
        }
    }

    public class MonNull : MonObj
    {
        public string Type()
        {
            return ObjectType.NULL_OBJ;
        }

        public string Inspect()
        {
            return "null";
        }
    }
}
