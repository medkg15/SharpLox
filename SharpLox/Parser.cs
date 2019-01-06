using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpLox
{
    public class Parser
    {
        private readonly List<Token> _tokens;
        private int current = 0;

        public Parser(List<Token> tokens)
        {
            _tokens = tokens;
        }

        public List<Statement> Parse()
        {
            try
            {
                var statements = new List<Statement>();
                while (!IsAtEnd())
                {
                    statements.Add(Declaration());
                }
                return statements;
            }
            catch (ParseError error)
            {
                return null;
            }
        }

        private Statement Declaration()
        {
            try
            {
                if (Match(TokenType.Var))
                {
                    return VarDeclaration();
                }
                return Statement();
            }
            catch (ParseError)
            {
                Synchronize();
                return null;
            }
        }

        private Statement VarDeclaration()
        {
            var name = Consume(TokenType.Identifier, "Expect variable name.");

            Expression initializer = null;
            if (Match(TokenType.Equal))
            {
                initializer = Expression();
            }

            Consume(TokenType.Semicolon, "Expect ';' after variable declaration");
            return new Statement.VarStatement(name, initializer);
        }

        private Statement Statement()
        {
            if (Match(TokenType.Print))
            {
                return PrintStatement();
            }

            return ExpressionStatement();
        }

        private Statement PrintStatement()
        {
            var value = Expression();
            Consume(TokenType.Semicolon, "Expect ';' after value.");
            return new Statement.PrintStatement(value);
        }

        private Statement ExpressionStatement()
        {
            var expression = Expression();
            Consume(TokenType.Semicolon, "Expect ';' after expression.");
            return new Statement.ExpressionStatement(expression);
        }

        private Expression Expression()
        {
            return Equality();
        }

        private Expression Equality()
        {
            var expression = Comparison();

            while (Match(TokenType.BangEqual, TokenType.EqualEqual))
            {
                var @operator = Previous();
                var right = Comparison();
                expression = new Expression.Binary(expression, @operator, right);
            }

            return expression;
        }

        private Expression Comparison()
        {
            var expression = Addition();

            while (Match(TokenType.Greater, TokenType.GreaterEqual, TokenType.Less, TokenType.LessEqual))
            {
                var @operator = Previous();
                var right = Addition();
                expression = new Expression.Binary(expression, @operator, right);
            }

            return expression;
        }

        private Expression Addition()
        {
            var expression = Multiplication();

            while (Match(TokenType.Minus, TokenType.Plus))
            {
                var @operator = Previous();
                var right = Multiplication();
                expression = new Expression.Binary(expression, @operator, right);
            }

            return expression;
        }

        private Expression Multiplication()
        {
            var expression = Unary();

            while (Match(TokenType.Star, TokenType.Slash))
            {
                var @operator = Previous();
                var right = Unary();
                expression = new Expression.Binary(expression, @operator, right);
            }

            return expression;
        }

        private Expression Unary()
        {
            if (Match(TokenType.Bang, TokenType.Minus))
            {
                return new Expression.Unary(Previous(), Unary());
            }

            return Primary();
        }

        private Expression Primary()
        {
            if (Match(TokenType.False))
            {
                return new Expression.Literal(false);
            }

            if (Match(TokenType.True))
            {
                return new Expression.Literal(true);
            }

            if (Match(TokenType.Nil))
            {
                return new Expression.Literal(null);
            }

            if (Match(TokenType.Number, TokenType.String))
            {
                return new Expression.Literal(Previous().Literal);
            }

            if (Match(TokenType.Identifier))
            {
                return new Expression.Variable(Previous());
            }

            if (Match(TokenType.LeftParen))
            {
                var expression = Expression();
                Consume(TokenType.RightParen, "Expect ')' after expression.");
                return new Expression.Grouping(expression);
            }

            throw Error(Peek(), "Expect expression.");
        }

        private void Synchronize()
        {
            Advance();

            while (!IsAtEnd())
            {
                if (Previous().Type == TokenType.Semicolon)
                {
                    return;
                }

                switch (Peek().Type)
                {
                    case TokenType.Class:
                    case TokenType.Fun:
                    case TokenType.Var:
                    case TokenType.For:
                    case TokenType.If:
                    case TokenType.While:
                    case TokenType.Print:
                    case TokenType.Return:
                        return;
                }

                Advance();
            }
        }

        private Token Consume(TokenType type, string message)
        {
            if (Check(type))
            {
                return Advance();
            }

            throw Error(Peek(), message);
        }

        private ParseError Error(Token token, string message)
        {
            Lox.Error(token, message);
            return new ParseError();
        }

        private bool Match(params TokenType[] types)
        {
            var match = types.Any(Check);
            if (match)
            {
                Advance();
            }
            return match;
        }

        private bool Check(TokenType type)
        {
            if (IsAtEnd())
            {
                return false;
            }
            return Peek().Type == type;
        }

        private Token Advance()
        {
            if (!IsAtEnd())
            {
                current++;
            }
            return Previous();
        }

        private bool IsAtEnd()
        {
            return Peek().Type == TokenType.Eof;
        }

        private Token Peek()
        {
            return _tokens[current];
        }

        private Token Previous()
        {
            return _tokens[current - 1];
        }

        public class ParseError : Exception
        {
            public ParseError() : base()
            {
            }

            public ParseError(string message) : base(message)
            {
            }

            public ParseError(string message, Exception innerException) : base(message, innerException)
            {
            }
        }
    }
}
