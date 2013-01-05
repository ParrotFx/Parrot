namespace Parrot.Nodes
{
    using System.ComponentModel;

    [Description("Raw output is for =identifier")]
    public class RawOutput : StringLiteral
    {
        public string VariableName { get; private set; }

        public RawOutput(string variableName) : this(variableName, null)
        {
        }

        public RawOutput(string variableName, StatementTail tail) : base("\"=" + variableName + "\"", tail)
        {
        }
    }
}