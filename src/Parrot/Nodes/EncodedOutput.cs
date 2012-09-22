using System.ComponentModel;
using Parrot.Infrastructure;

namespace Parrot.Nodes
{
    [Description("Encoded output is for : identifier")]
    public class EncodedOutput : Statement
    {
        public string VariableName { get; private set; }

        public EncodedOutput(IHost host, string variableName) : base(host)
        {
            Name = "output";
            VariableName = variableName;
        }

        public override bool IsTerminal
        {
            get { return true; }
        }

    }
}