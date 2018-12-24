using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SharpLox
{
    public class Scanner
    {
        private static readonly IDictionary<string, TokenType> Keywords = new Dictionary<string, TokenType>
        {
            { "and", TokenType.And },
            { "class", TokenType.Class },
            { "else", TokenType.Else },
            { "false", TokenType.False },
            { "for", TokenType.For },
            { "fun", TokenType.Fun },
            { "if", TokenType.If },
            { "nil", TokenType.Nil },
            { "or", TokenType.Or },
            { "print", TokenType.Print },
            { "return", TokenType.Return },
            { "super", TokenType.Super },
            { "this", TokenType.This },
            { "true", TokenType.True },
            { "var", TokenType.Var },
            { "while", TokenType.While },
        };

        private readonly string _source;
        private readonly List<Token> _tokens = new List<Token>();

        private int start = 0;
        private int current = 0;
        private int line = 1;

        public Scanner(string source)
        {
            _source = source;
        }

        public List<Token> ScanTokens()
        {
            while (!IsAtEnd())
            {
                start = current;
                ScanToken();
            }

            _tokens.Add(new Token(TokenType.Eof, "", null, line));
            return _tokens;
        }

        private void ScanToken()
        {
            char c = Advance();
            switch (c)
            {
                case '(': AddToken(TokenType.LeftParen); break;
                case ')': AddToken(TokenType.RightParen); break;
                case '{': AddToken(TokenType.LeftBrace); break;
                case '}': AddToken(TokenType.RightBrace); break;
                case ',': AddToken(TokenType.Comma); break;
                case '.': AddToken(TokenType.Dot); break;
                case '-': AddToken(TokenType.Minus); break;
                case '+': AddToken(TokenType.Plus); break;
                case ';': AddToken(TokenType.Semicolon); break;
                case '*': AddToken(TokenType.Star); break;
                case '!': AddToken(Match('=') ? TokenType.BangEqual : TokenType.Bang); break;
                case '>': AddToken(Match('=') ? TokenType.GreaterEqual : TokenType.Greater); break;
                case '<': AddToken(Match('=') ? TokenType.LessEqual : TokenType.Less); break;
                case '=': AddToken(Match('=') ? TokenType.EqualEqual : TokenType.Equal); break;
                case '/':
                    if (!MatchComment())
                    {
                        AddToken(TokenType.Slash);
                    };
                    break;
                case '"':
                    ConsumeString(); break;
                case ' ':
                case '\r':
                case '\t':
                    // ignore whitespace
                    break;
                case '\n':
                    line++;
                    break;
                default:
                    if (IsDigit(c))
                    {
                        ConsumeNumber();
                    }
                    else if (IsAlpha(c))
                    {
                        ConsumeIdentifier();
                    }
                    else
                    {
                        Program.Error(line, "Unexpected character.");
                    }
                    break;
            }
        }

        private char Advance()
        {
            current++;
            return _source[current - 1];
        }

        private char Peek()
        {
            if (IsAtEnd())
            {
                return '\0';
            }
            return _source[current];
        }

        private char PeekNext()
        {
            if (current + 1 >= _source.Length)
            {
                return '\0';
            }
            return _source[current + 1];
        }

        private void AddToken(TokenType type)
        {
            AddToken(type, null);
        }

        private void AddToken(TokenType type, object literal)
        {
            var text = _source.Substring(start, current - start);
            _tokens.Add(new Token(type, text, literal, line));
        }

        private bool IsAtEnd()
        {
            return current >= _source.Length;
        }

        private bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        private bool IsAlpha(char c)
        {
            return (c >= 'a' && c <= 'z')
                || (c >= 'A' && c <= 'Z')
                || c == '_';
        }

        private bool IsAlphaNumeric(char c)
        {
            return IsAlpha(c) || IsDigit(c);
        }

        private bool Match(char c)
        {
            if (IsAtEnd())
            {
                return false;
            }

            if (_source[current] != c)
            {
                return false;
            }

            current++;
            return true;
        }

        private bool MatchComment()
        {
            // single line comment
            if (Match('/'))
            {
                while (Peek() != '\n' && !IsAtEnd())
                {
                    Advance();
                }
                return true;
            }

            // block comments
            if (Match('*'))
            {
                var foundEnd = false;
                while (!foundEnd && !IsAtEnd())
                {
                    var next = Advance();
                    if (next == '\n')
                    {
                        line++;
                    }
                    if (next == '*')
                    {
                        if (Peek() == '/')
                        {
                            foundEnd = true;
                            current++;
                        }
                    }
                }

                if (!foundEnd)
                {
                    Program.Error(line, "Unclosed comment block.");
                }

                return true;
            }

            return false;
        }

        private void ConsumeNumber()
        {
            while (IsDigit(Peek()) && !IsAtEnd())
            {
                Advance();
            }

            // check if we have a number after a period.
            if (Peek() == '.' && IsDigit(PeekNext()))
            {
                Advance(); // consume .

                while (IsDigit(Peek()))
                {
                    Advance();
                }
            }

            AddToken(TokenType.Number, Double.Parse(_source.Substring(start, current - start)));
        }

        private void ConsumeString()
        {
            while (!IsAtEnd())
            {
                if (Peek() == '"')
                {
                    current++;
                    AddToken(TokenType.String, _source.Substring(start + 1, current - start - 2));
                    return;
                }

                if (Peek() == '\n')
                {
                    Program.Error(line, "Unclosed string literal.");
                    return;
                }

                Advance();
            }
            Program.Error(line, "Unclosed string literal.");
        }

        private void ConsumeIdentifier()
        {
            while (IsAlphaNumeric(Peek()))
            {
                Advance();
            }

            var text = _source.Substring(start, current - start);

            if (Keywords.ContainsKey(text))
            {
                AddToken(Keywords[text]);
            }
            else
            {
                AddToken(TokenType.Identifier);
            }
        }
    }
}
