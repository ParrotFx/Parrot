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
        private static Lazy<IDictionary<string, Func<string, ValueTypeResult>>> keywordHandlers = new Lazy<IDictionary<string, Func<string, ValueTypeResult>>>(() => InitializeKeywordHanlders());

        private static IDictionary<string, Func<string, ValueTypeResult>> InitializeKeywordHanlders()
        {
            var handlers = new Dictionary<string, Func<string, ValueTypeResult>>();
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

            if (IsWrappedInInvalidQuotes(value))
            {
                throw new ParserException("Unterminated string literal");
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
                if (keywordHandlers.Value.ContainsKey(value))
                {
                    result = keywordHandlers.Value[value](value);
                }
                else
                {
                    result.Type = ValueType.Property;
                    result.Value = value;
                }
            }

            return result;
        }

        private bool IsWrappedInQuotes(string value)
        {
            return (value.StartsWith("\"") && value.EndsWith("\"")) || (value.StartsWith("'") || value.EndsWith("'"));
        }

        private bool IsWrappedInInvalidQuotes(string value)
        {
            return value != null && (((value.StartsWith("\"") && !value.EndsWith("\"")) || (value.StartsWith("'") && !value.EndsWith("'")))
                                 || ((!value.StartsWith("\"") && value.EndsWith("\"")) || (!value.StartsWith("'") && value.EndsWith("'"))));
        }

    }

    public interface IValueTypeProvider
    {
        ValueTypeResult GetValue(string value);
    }
}
