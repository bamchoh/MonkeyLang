using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using MonkeyLang.Lexer;
using MonkeyLang.Token;

namespace MonkyLangREPL.Repl
{
    class Repl
    {
        static readonly string PROMPT = ">> ";

        public static void Start(TextReader tr, TextWriter tw)
        {
            while(true)
            {
                Console.Write(PROMPT);
                var line = tr.ReadLine();
                if(line == null) {
                    return;
                }

                var l = new Lexer(line);

                for(var tok = l.NextToken(); tok.Type != TokenTypes.EOF; tok = l.NextToken())
                {
                    Console.WriteLine("Type:{0}, Literal:{1}", tok.Type, tok.Literal);
                }
            }
        }
    }
}
