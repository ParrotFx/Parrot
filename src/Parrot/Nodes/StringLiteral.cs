// -----------------------------------------------------------------------
// <copyright file="StringLiteralNode.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Parrot.Nodes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class StringLiteral : Statement
    {
        public string Value { get; private set; }
        public ValueType ValueType { get; private set; }

        public StringLiteral(string value)
            : base("string")
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

            Value = value;
        }

        public object GetValue()
        {

            if (ValueType == ValueType.Property)
            {
                var value = GetModelValue(Value);
                return value;
            }

            if (ValueType == ValueType.Keyword)
            {
                switch(Value)
                {
                    case "null":
                        return null;
                    case "false":
                        return false;
                    case "true":
                        return true;
                }
            }

            return Value;
        }

        private bool IsWrappedInQuotes(string value)
        {
            return ((value.StartsWith("\"")|| value.StartsWith("@\"")) && value.EndsWith("\"")) || ((value.StartsWith("'")||value.StartsWith("@'")) && value.EndsWith("'"));
        }

        public override bool IsTerminal
        {
            get { return true; }
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
