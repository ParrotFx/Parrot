using System.ComponentModel;
using Parrot.Infrastructure;

namespace Parrot.Nodes
{
    [Description("Raw output is for = identifier")]
    public class RawOutput : Statement
    {
        public string VariableName { get; set; }

        public RawOutput(IHost host, string variableName) : base(host)
        {
            Name = "rawoutput";
            VariableName = variableName;
        }

        public override bool IsTerminal
        {
            get { return true; }
        }

    }
}