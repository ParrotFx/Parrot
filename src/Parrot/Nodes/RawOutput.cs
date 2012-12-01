using System.ComponentModel;
using Parrot.Infrastructure;

namespace Parrot.Nodes
{
    [Description("Raw output is for =identifier")]
    public class RawOutput : StringLiteral
    {
        public string VariableName { get; private set; }

        public RawOutput(IHost host, string variableName) : this(host, variableName, null)
        {
        }

        public RawOutput(IHost host, string variableName, StatementTail tail) : base(host, "\"=" + variableName + "\"", tail)
        {
        }
    }
}