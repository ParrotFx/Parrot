namespace Parrot.Nodes
{
    public class EncodedOutput : Statement
    {
        public string VariableName { get; private set; }

        public EncodedOutput(string variableName) : base("output")
        {
            VariableName = variableName;
        }

        public override bool IsTerminal
        {
            get { return true; }
        }

    }
}