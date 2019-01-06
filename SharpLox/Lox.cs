using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpLox
{
    public static class Lox
    {
        private static bool hadError;
        private static bool hadRuntimeError;
        private static readonly Interpreter interpreter = new Interpreter();

        static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine("Usage: sharplox [script]");
                System.Environment.Exit(64);
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
            var script = File.ReadAllText(Path.Combine(System.Environment.CurrentDirectory, filename));
            Run(script);
            if (hadError)
            {
                System.Environment.Exit(65);
            }
            if (hadRuntimeError)
            {
                System.Environment.Exit(70);
            }
        }

        private static void RunPrompt()
        {
            while (true)
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
            var scanner = new Scanner(script);
            var tokens = scanner.ScanTokens();
            var parser = new Parser(tokens);
            var statements = parser.Parse();

            if (hadError)
            {
                return;
            }

            interpreter.Interpret(statements);
        }

        public static void Error(int line, string message)
        {
            Report(line, "", message);
            hadError = true;
        }

        public static void Error(Token token, string message)
        {
            if (token.Type == TokenType.Eof)
            {
                Report(token.Line, " at end", message);
            }
            else
            {
                Report(token.Line, $" at '{token.Lexeme}'", message);
            }
            hadError = true;
        }

        public static void RuntimeError(RuntimeError error)
        {
            Console.WriteLine($"{error.Message} \n[line {error.Token.Line}]");
            hadRuntimeError = true;
        }

        private static void Report(int line, string where, string message)
        {
            Console.WriteLine($"[line {line}] Error {where}: {message}");
        }
    }
}
