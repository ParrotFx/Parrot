// -----------------------------------------------------------------------
// <copyright file="ValueTypeProvider.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Parrot.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ValueTypeProvider : IValueTypeProvider
    {
        private static readonly Lazy<IDictionary<string, Func<string, ValueTypeResult>>> KeywordHandlers = new Lazy<IDictionary<string, Func<string, ValueTypeResult>>>(InitializeKeywordHanlders);

        private static IDictionary<string, Func<string, ValueTypeResult>> InitializeKeywordHanlders()
        {
            var handlers = new Dictionary<string, Func<string, ValueTypeResult>>(4);
            handlers.Add("this", s => new ValueTypeResult { Type = ValueType.Local, Value = "this" });
            handlers.Add("false", s => new ValueTypeResult { Type = ValueType.Keyword, Value = false });
            handlers.Add("true", s => new ValueTypeResult { Type = ValueType.Keyword, Value = true });
            handlers.Add("null", s => new ValueTypeResult { Type = ValueType.Keyword, Value = null });

            return handlers;
        }

        public ValueTypeResult GetValue(string value)
        {
            var result = new ValueTypeResult();

            if (value == null)
            {
                return new ValueTypeResult
                {
                    Type = ValueType.StringLiteral,
                    Value = null
                };
            }
            
            if (IsWrappedInQuotes(value))
            {
                result.Type = ValueType.StringLiteral;
                //strip quotes
                result.Value = value.Substring(1, value.Length - 2);
            }
            else
            {
                //check for keywords
                if (KeywordHandlers.Value.ContainsKey(value))
                {
                    result = KeywordHandlers.Value[value](value);
                }
                else
                {
                    result.Type = ValueType.Property;
                    result.Value = value;
                }
            }

            return result;
        }

        private static bool EndsWith(string source, char value)
        {
            int length = source.Length;
            return length != 0 && source[length - 1] == value;
        }

        private static bool StartsWith(string source, char value)
        {
            return source.Length > 0 && source[0] == value;
        }

        private bool IsWrappedInQuotes(string value)
        {
            return (StartsWith(value, '"') && EndsWith(value, '"')) || (StartsWith(value, '\'') || EndsWith(value, '\''));
        }

    }

    public interface IValueTypeProvider
    {
        ValueTypeResult GetValue(string value);
    }
}
