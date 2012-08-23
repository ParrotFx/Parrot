using Parrot.Infrastructure;

namespace Parrot.Nodes
{
    public class EncodedOutput : Statement
    {
        public string VariableName { get; private set; }

        public EncodedOutput(IHost host, string variableName) : base(host, "output")
        {
            VariableName = variableName;
        }

        public override bool IsTerminal
        {
            get { return true; }
        }

    }
}