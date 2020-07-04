using System;

namespace MonkyLangREPL
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello {0}! This is the Monkey programming language!", Environment.UserName);
            Console.WriteLine("Feel free to type in commands");
            Repl.Repl.Start(Console.In, Console.Out);
        }
    }
}
