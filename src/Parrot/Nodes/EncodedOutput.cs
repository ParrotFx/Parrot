using System.ComponentModel;
using Parrot.Infrastructure;

namespace Parrot.Nodes
{
    [Description("Encoded output is for @ identifier")]
    public class EncodedOutput : StringLiteral 
    {
        public string VariableName { get; private set; }
        //public ValueType ValueType { get; private set; }

        public EncodedOutput(IHost host, string variableName) : this(host, variableName, null)
        {
        }

        public EncodedOutput(IHost host, string variableName, StatementTail tail) : base(host, "\"@" + variableName + "\"", tail)
        {
        }
    }
}