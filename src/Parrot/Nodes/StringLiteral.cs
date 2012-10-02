// -----------------------------------------------------------------------
// <copyright file="StringLiteralNode.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Parrot.Infrastructure;
using ValueType = Parrot.Infrastructure.ValueType;

namespace Parrot.Nodes
{
    public class StringLiteral : Statement
    {
        public List<StringLiteralPart> Values { get; private set; }
        public ValueType ValueType { get; private set; }

        public StringLiteral(IHost host, string value) : this(host, value, null) { }

        public StringLiteral(IHost host, string value, StatementTail tail) : base(host, "string", tail)
        {
            if (IsWrappedInQuotes(value))
            {
                ValueType = ValueType.StringLiteral;
                //strip quotes

                int offset = StartsWith(value, '@') ? 2 : 1;
                value = new string(value.ToArray(), offset, value.Length - (offset + 1));
            }
            else if (value == "this")
            {
                ValueType = ValueType.Local;
            }
            else if (value == "null" || value == "true" || value == "false")
            {
                ValueType = ValueType.Keyword;
            }
            else
            {
                ValueType = ValueType.Property;
            }

            if (ValueType == ValueType.StringLiteral)
            {
                Values = Parse(value);
            }
        }

        private static bool StartsWith(string source, char value)
        {
            return source.Length > 0 && source[0] == value;
        }

        private bool IsWrappedInQuotes(string value)
        {
            return (StartsWith(value, '"') || StartsWith(value, '\''));
        }
        
        public override bool IsTerminal
        {
            get { return true; }
        }

        public override string ToString()
        {
            return string.Join("", Values.Select(f => f.Data));
        }

        private List<StringLiteralPart> Parse(string source)
        {
            List<StringLiteralPart> parts = new List<StringLiteralPart>(128);

            int tempCounter = 0;
            char[] c = new char[source.Length];

            for (int i = 0; i < source.Length; i++)
            {
                if (source[i] == ':' || source[i] == '=')
                {
                    char comparer = source[i];

                    i += 1;
                    //look ahead by 1
                    if (source[Math.Min(source.Length - 1, i)] == comparer)
                    {
                        //it's a single ":" escaped
                        c[tempCounter++] = comparer;
                    }
                    else if (IsIdentifierHead(source[i]))
                    {
                        //build a new word

                        if (tempCounter > 0)
                        {
                            parts.Add(new StringLiteralPart(StringLiteralPartType.Literal, new string(c, 0, tempCounter), i - tempCounter));
                        }

                        tempCounter = 0;

                        var word = new StringBuilder();
                        //check for non-identifier character
                        //read until non-identifier character
                        for (; i < source.Length; i++, tempCounter++)
                        {
                            if (!IsIdTail(source[i]))
                            {
                                break;
                            }

                            word.Append(source[i]);
                        }

                        if (word[word.Length - 1] == '.')
                        {
                            word.Length -= 1;
                            parts.Add(new StringLiteralPart(comparer == ':' ? StringLiteralPartType.Encoded : StringLiteralPartType.Raw, word.ToString(), i - tempCounter));
                            tempCounter = 0;
                            c[tempCounter++] = '.';
                        }
                        else
                        {
                            parts.Add(new StringLiteralPart(comparer == ':' ? StringLiteralPartType.Encoded : StringLiteralPartType.Raw, word.ToString(), i - tempCounter));
                            tempCounter = 0;
                        }

                        if (i < source.Length)
                        {
                            c[tempCounter++] = source[i];
                        }
                    }
                    else
                    {
                        c[tempCounter++] = comparer;
                        c[tempCounter++] = source[i];
                    }
                }
                else
                {
                    c[tempCounter++] = source[i];
                }
            }

            if (tempCounter > 0)
            {
                parts.Add(new StringLiteralPart(StringLiteralPartType.Literal, new string(c, 0, tempCounter), source.Length - tempCounter));
            }

            return parts;
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

    }
}
