namespace Parrot.Nodes
{
    using Parrot.Nodes;

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

        public override string ToString()
        {
            var value = GetModelValue(VariableName);
            if (value != null)
            {
                return System.Net.WebUtility.HtmlEncode(GetModelValue(VariableName).ToString());
            }

            return null;
        }
    }
}