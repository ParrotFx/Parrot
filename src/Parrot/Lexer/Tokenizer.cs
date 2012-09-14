using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Parrot.Lexer
{
    public class Tokenizer
    {
        private readonly Stack<Token> _tokens = new Stack<Token>();
        private int _currentIndex;
        private readonly StreamReader _reader;

        public Tokenizer(string source) : this(new MemoryStream(Encoding.Default.GetBytes(source))) { }

        public Tokenizer(Stream source)
        {
            _reader = new StreamReader(source);
        }

        private bool HasAvailableTokens()
        {
            return _reader.Peek() != -1;
        }

        private int Consume()
        {
            _currentIndex += 1;
            var character = _reader.Read();

            if (character == -1)
            {
                throw new EndOfStreamException();
            }

            return character;
        }

        private Token GetNextToken()
        {
            int peek = _reader.Peek();
            var currentCharacter = peek == -1 ? '\0' : (char)peek;

            if (IsIdentifierHead(currentCharacter))
            {
                return new IdentifierToken
                {
                    Content = ReadUntil(c => !IsIdTail(c)),
                    Index = _currentIndex,
                    Type = TokenType.Identifier
                };
            }
            
            if (IsWhitespace(currentCharacter))
            {
                return new WhitespaceToken
                {
                    Content = ReadUntil(c => !IsWhitespace(c)),
                    Index = _currentIndex,
                    Type = TokenType.Whitespace
                };
            }

            switch (currentCharacter)
            {
                case ',': //this is for the future
                    Consume();
                    return new CommaToken { Index = _currentIndex };
                case '(': //parameter list start
                    Consume();
                    return new OpenParenthesisToken { Index = _currentIndex };
                case ')': //parameter list end
                    Consume();
                    return new CloseParenthesisToken { Index = _currentIndex };
                case '[': //attribute list start
                    Consume();
                    return new OpenBracketToken { Index = _currentIndex };
                case ']': //attribute list end
                    Consume();
                    return new CloseBracketToken { Index = _currentIndex };
                case '=': //attribute assignment, raw output
                    Consume();
                    return new EqualToken { Index = _currentIndex };
                case '{': //child block start
                    Consume();
                    return new OpenBracesToken { Index = _currentIndex };
                case '}': //child block end
                    Consume();
                    return new CloseBracesToken { Index = _currentIndex };
                case '>': //child assignment
                    Consume();
                    return new GreaterThanToken { Index = _currentIndex };
                case '+': //sibling assignment
                    Consume();
                    return new PlusToken { Index = _currentIndex };
                case '|': //string literal pipe
                    return new StringLiteralPipeToken
                    {
                        Content = (char)Consume() + ReadUntil(IsNewLine),
                        Type = TokenType.StringLiteralPipe,
                        Index = _currentIndex
                    };
                case '"': //quoted string literal
                    return new QuotedStringLiteralToken
                    {
                        Content = (char)Consume() + ReadUntil(c => IsNewLine(c) || c == '"') + (char)Consume(),
                        Type = TokenType.QuotedStringLiteral,
                        Index = _currentIndex
                    };
                case '\'': //quoted string literal
                    return new QuotedStringLiteralToken
                    {
                        Content = (char)Consume() + ReadUntil(c => IsNewLine(c) || c == '\'') + (char)Consume(),
                        Type = TokenType.QuotedStringLiteral,
                        Index = _currentIndex
                    };
                case '@': //multilinestringliteral
                    //read next token
                    Consume();
                    int nextCharacter = _reader.Peek();
                    char quoteType = nextCharacter == -1 ? '\0' : (char)nextCharacter;
                    return new MultilineStringLiteralToken
                    {
                        Content = (char)Consume() + ReadUntil(c => IsNewLine(c) || c == nextCharacter) + (char)Consume(),
                        Type = TokenType.StringLiteralPipe,
                        Index = _currentIndex
                    };
                case ':': //Encoded output
                    Consume();
                    return new ColonToken { Index = _currentIndex };
                default:
                    throw new UnexpectedTokenException(string.Format("Unexpected token: {0}", currentCharacter));
            }
        }

        private bool IsWhitespace(char character)
        {
            return
                   character == '\r' ||
                   character == '\n' ||
                   character == ' ' ||
                   character == '\f' ||
                   character == '\t' ||
                   character == '\u000B' || // Vertical Tab
                   Char.GetUnicodeCategory(character) == UnicodeCategory.SpaceSeparator;
        }

        private string ReadUntil(Func<char, bool> until)
        {
            List<char> result = new List<char>();
            int peek = _reader.Peek();
            var currentCharacter = peek == -1 ? '\0' : (char)peek;
            while (!until(currentCharacter))
            {
                Consume();
                result.Add(currentCharacter);
                peek = _reader.Peek();
                currentCharacter = peek == -1 ? '\0' : (char)peek;
            }

            return string.Join("", result);
        }

        private bool IsIdentifierHead(char character)
        {
            return Char.IsLetter(character) ||
                   character == '_' ||
                   character == '#' ||
                   character == '.' ||
                   Char.GetUnicodeCategory(character) == UnicodeCategory.LetterNumber;
        }

        private bool IsIdTail(char character)
        {
            return Char.IsDigit(character) ||
                   IsIdentifierHead(character) ||
                   character == ':' ||
                   character == '-' ||
                   character == '.' ||
                   IsIdentifierUnicode(character);
        }

        private bool IsIdentifierUnicode(char character)
        {
            UnicodeCategory category = Char.GetUnicodeCategory(character);

            return category == UnicodeCategory.NonSpacingMark ||
                   category == UnicodeCategory.SpacingCombiningMark ||
                   category == UnicodeCategory.ConnectorPunctuation ||
                   category == UnicodeCategory.Format;
        }

        private bool IsNewLine(char character)
        {
            return character == '\r' // Carriage return
                || character == '\n' // Linefeed
                || character == '\u0085' // Next Line
                || character == '\u2028' // Line separator
                || character == '\u2029'; // Paragraph separator
        }

        public Stack<Token> Tokenize()
        {
            while (HasAvailableTokens())
            {
                //gonna yield these tokens later
                _tokens.Push(GetNextToken());
            }

            return _tokens;
        }

        public IEnumerable<Token> Tokens()
        {
            while (HasAvailableTokens())
            {
                yield return GetNextToken();
            }
        }
    }
}
