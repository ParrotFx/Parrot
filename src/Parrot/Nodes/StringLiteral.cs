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

        public StringLiteral(IHost host, string value) : base(host, "string")
        {
            if (IsWrappedInQuotes(value))
            {
                ValueType = ValueType.StringLiteral;
                //strip quotes
                value = value.Substring(value.StartsWith("@") ? 2 : 1, value.Length - (value.StartsWith("@") ? 3 : 2));
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
                //what to do with this
            }

        }

        private bool IsWrappedInQuotes(string value)
        {
            return ((value.StartsWith("\"") || value.StartsWith("@\"")) && value.EndsWith("\"")) || ((value.StartsWith("'") || value.StartsWith("@'")) && value.EndsWith("'"));
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
            //valid characters
            //:_abcdefgh1234567890
            //_a-z

            //example identifiers because unicode is allowed... :(
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
                    else if (IsIdentifierHead(source[i])) //.Contains(source[i]))
                    {
                        //build a new word

                        if (tempCounter > 0)
                        {
                            parts.Add(new StringLiteralPart(StringLiteralPartType.Literal, string.Join("", c.Take(tempCounter).ToArray()), i - tempCounter));
                        }

                        tempCounter = 0;

                        var word = new StringBuilder();
                        //check for non-identifier character
                        //read until non-identifier character
                        for (; i < source.Length; i++, tempCounter++)
                        {
                            if (!IsIdTail(source[i]))
                            //if (!_idFooter.Contains(source[i]))
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
                        //

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
                parts.Add(new StringLiteralPart(StringLiteralPartType.Literal, string.Join("", c.Take(tempCounter).ToArray()), source.Length - tempCounter));
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
