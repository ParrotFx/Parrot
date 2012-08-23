using Parrot.Infrastructure;

namespace Parrot.Nodes
{
    public class RawOutput : Statement
    {
        public string VariableName { get; set; }

        public RawOutput(IHost host, string variableName) : base(host, "rawoutput")
        {
            VariableName = variableName;
        }

        public override bool IsTerminal
        {
            get { return true; }
        }

    }
}