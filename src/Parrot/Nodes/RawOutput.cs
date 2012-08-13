namespace Parrot.Nodes
{
    public class RawOutput : Statement
    {
        public string VariableName { get; set; }

        public RawOutput(string variableName) : base("rawoutput")
        {
            VariableName = variableName;
        }

        public override bool IsTerminal
        {
            get { return true; }
        }

    }
}