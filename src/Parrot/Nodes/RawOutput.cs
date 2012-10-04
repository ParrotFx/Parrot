using System.ComponentModel;
using Parrot.Infrastructure;

namespace Parrot.Nodes
{
    [Description("Raw output is for =identifier")]
    public class RawOutput : StringLiteral
    {
        public string VariableName { get; private set; }
        //public ValueType ValueType { get; private set; }

        public RawOutput(IHost host, string variableName)
            : this(host, variableName, null)
        {
        }

        public RawOutput(IHost host, string variableName, StatementTail tail)
            : base(host, "\"=" + variableName + "\"", tail)
        {
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

    }
}