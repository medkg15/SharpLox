using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpLox
{
    public class Program
    {
        private static bool hadError;

        static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine("Usage: sharplox [script]");
                Environment.Exit(64);
            }
            else if (args.Length == 1)
            {
                RunFile(args[0]);
            }
            else
            {
                RunPrompt();
            }
        }

        private static void RunFile(string filename)
        {
            var script = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, filename));
            Run(script);
            if (hadError)
            {
                Environment.Exit(65);
            }
        }

        private static void RunPrompt()
        {
            while(true)
            {
                Console.Write("> ");
                var input = Console.ReadLine();
                if (input == "exit")
                {
                    break;
                }
                Run(input);
                hadError = false;
            }
        }

        private static void Run(string script)
        {
            var scanner = new Scanner();
            var tokens = scanner.ScanTokens(script);

            foreach(var token in tokens)
            {
                Console.WriteLine(token);
            }
        }

        private static void Error(int line, string message)
        {
            Report(line, "", message);
        }

        private static void Report(int line, string where, string message)
        {
            Console.WriteLine($"[line {line}] Error {where}: {message}");
            hadError = true;
        }
    }
}
