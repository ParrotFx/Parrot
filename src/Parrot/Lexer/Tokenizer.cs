namespace Parrot.Lexer
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text;

    public class Tokenizer
    {
        private readonly List<Token> _tokens = new List<Token>();
        private int _currentIndex;
        private readonly StreamReader _reader;

        private static readonly HashSet<char> _whitespaceChars = new HashSet<char>(new[] { '\r', '\n', ' ', '\f', '\t', '\u000B' });
        private static readonly HashSet<char> _newLineChars = new HashSet<char>(new[]
            {
                '\r', // Carriage return
                '\n', // Linefeed
                '\u0085', // Next Line
                '\u2028', // Line separator
                '\u2029' // Paragraph separator
            });

        private static readonly HashSet<char> _tokenChars = new HashSet<char>(new[]
            {
                ',', //this is for the future
                '(', //parameter list start
                ')', //parameter list end
                '[', //attribute list start
                ']', //attribute list end
                '=', //attribute assignment, raw output
                '{', //child block start
                '}', //child block end
                '>', //child assignment
                '+', //sibling assignment
                '@', //Encoded output
                '^', //Climb up token
            });

        public Tokenizer(Stream source)
        {
            _reader = new StreamReader(source);
        }

        private char Consume()
        {
            _currentIndex += 1;
            int character = _reader.Read();

            if (character == -1)
            {
                throw new EndOfStreamException();
            }

            return (char) character;
        }

        private Token GetNextToken()
        {
            char currentCharacter = PeekCurrentCharacter();

            if (IsIdentifierHead(currentCharacter))
            {
                return ConsumeToken(TokenType.Identifier, ConsumeIdentifier);
            }

            if (IsWhitespace(currentCharacter))
            {
                return ConsumeToken(TokenType.Whitespace, ConsumeWhitespace);
            }

            if (IsSingleCharToken(currentCharacter))
            {
                return ConsumeSingleCharToken(currentCharacter);
            }

            switch (currentCharacter)
            {
                case '|': //string literal pipe
                    return ConsumeMultiCharToken(currentCharacter, ConsumeUntilNewlineOrEndOfStream);
                case '"': //quoted string literal
                case '\'': //quoted string literal
                    return ConsumeMultiCharToken(currentCharacter, () => ConsumeQuotedStringLiteral(currentCharacter));
                case '\0':
                    return null;
                default:
                    throw new UnexpectedTokenException(string.Format("Unexpected token: {0}", currentCharacter));
            }
        }

        private static bool IsSingleCharToken(char currentCharacter)
        {
            return _tokenChars.Contains(currentCharacter);
        }

        private Token ConsumeMultiCharToken(char currentCharacter, Func<string> contentFunc)
        {
            Token token = TokenFactory.Create(currentCharacter);
            return initToken(token, contentFunc);
        }

        private Token initToken(Token token, Func<string> contentFunc)
        {
            token.Index = _currentIndex;
            token.Content = contentFunc();
            return token;
        }

        private Token ConsumeSingleCharToken(char currentCharacter)
        {
            Token token = TokenFactory.Create(currentCharacter);
            token.Index = _currentIndex;
            token.Content = Consume().ToString();
            return token;
        }

        private Token ConsumeToken(TokenType type, Func<string> contentFunc)
        {
            Token token = TokenFactory.Create(type);
            return initToken(token, contentFunc);
        }

        private string ConsumeUntilNewlineOrEndOfStream()
        {
            //(char)Consume() + ReadUntil(IsNewLine)
            var sb = new StringBuilder();
            char currentCharacter = PeekCurrentCharacter();

            while (!IsNewLine(currentCharacter))
            {
                try
                {
                    Consume();
                    sb.Append(currentCharacter);
                    currentCharacter = PeekCurrentCharacter();
                }
                catch (EndOfStreamException)
                {
                    //end of stream exception
                    //we're at the end of the line return
                    break;
                }
            }

            return sb.ToString();
        }

        private char PeekCurrentCharacter()
        {
            int peek = _reader.Peek();
            return peek == -1 ? '\0' : (char)peek;
        }

        private string ConsumeIdentifier()
        {
            var sb = new StringBuilder();
            char currentCharacter = PeekCurrentCharacter();

            while ((IsIdTail(currentCharacter)))
            {
                Consume();
                sb.Append(currentCharacter);
                int peek = _reader.Peek();
                currentCharacter = peek == -1 ? '\0' : (char)peek;
            }

            return sb.ToString();
        }

        private string ConsumeWhitespace()
        {
            var sb = new StringBuilder();
            char currentCharacter = PeekCurrentCharacter();

            while ((IsWhitespace(currentCharacter)))
            {
                Consume();
                sb.Append(currentCharacter);
                currentCharacter = PeekCurrentCharacter();
            }

            return sb.ToString();
        }

        private string ConsumeQuotedStringLiteral(char quote)
        {
            var sb = new StringBuilder();
            sb.Append(Consume());
            char currentCharacter = PeekCurrentCharacter();

            //extra quote for continuations

            while (true)
            {
                while (currentCharacter != quote)
                {
                    Consume();
                    sb.Append(currentCharacter);
                    currentCharacter = PeekCurrentCharacter();
                }
                sb.Append(Consume());
                if (_reader.Peek() != quote)
                {
                    break;
                }
                Consume();
                currentCharacter = PeekCurrentCharacter();
            }

            //sb.Append((char) Consume());
            return sb.ToString();
        }

        private bool IsWhitespace(char character)
        {
            return _whitespaceChars.Contains(character) || Char.GetUnicodeCategory(character) == UnicodeCategory.SpaceSeparator;
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
            return _newLineChars.Contains(character);
        }

        private List<Token> Tokenize()
        {
            Token token;
            while ((token = GetNextToken()) != null)
            {
                _tokens.Add(token);
            }
            return _tokens;
        }

        public IList<Token> Tokens()
        {
            return Tokenize();
        }
    }
}