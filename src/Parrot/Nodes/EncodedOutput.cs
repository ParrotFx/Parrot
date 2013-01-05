namespace Parrot.Nodes
{
    using System.ComponentModel;

    [Description("Encoded output is for @ identifier")]
    public class EncodedOutput : StringLiteral
    {
        public string VariableName { get; private set; }
        //public ValueType ValueType { get; private set; }

        public EncodedOutput(string variableName) : this(variableName, null)
        {
        }

        public EncodedOutput(string variableName, StatementTail tail) : base("\"@" + variableName + "\"", tail)
        {
        }
    }
}