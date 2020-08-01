using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using MonkeyLang.Lexer;
using MonkeyLang.Token;
using MonkeyLang.Parser;

namespace MonkyLangREPL.Repl
{
    class Repl
    {
        static readonly string PROMPT = ">> ";

        const string MONKEY_FACE = @"
           --,--
  .--.  .-'     '-.  .--.
 / .. \/  .-. .-.  \/ .. \
| |  '|  /   Y   \  |'  | |
| \   \  \ 0 | 0 /  /'  / |
 \ '- ,\.-^^^^^^^-./, -' /
  ''-' /_   ^ ^   _\ '-''
      |   \._ _./   |
      \   \ '~' /   /
       '._ '-=-' _.'
          '-----'
";

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
                var p = new Parser(l);

                var program = p.ParserProgram();
                if(p.Errors().Count != 0)
                {
                    printParserErrors(tw, p.Errors());
                    continue;
                }

                tw.Write(program.String());
                tw.WriteLine();
            }
        }

        private static void printParserErrors(TextWriter tw, List<string> errors)
        {
            tw.Write(MONKEY_FACE);
            tw.WriteLine("Woops! We ran into some monkey business here!");
            tw.WriteLine(" parser errors:");
            foreach(var msg in errors)
            {
                tw.WriteLine(string.Format("\t{0}", msg));
            }
        }
    }
}
