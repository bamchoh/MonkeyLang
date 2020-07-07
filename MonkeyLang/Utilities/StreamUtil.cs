using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MonkeyLang.Utilities
{
    public class StringBuffer
    {
        MemoryStream ms = new MemoryStream();

        public void Write(string s)
        {
            ms.Write(Encoding.UTF8.GetBytes(s));
        }

        public string String()
        {
            return Encoding.UTF8.GetString(ms.ToArray());
        }
    }
}
