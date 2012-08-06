namespace Parrot.Nodes
{
    using System.Reflection;
    using Parrot.Nodes;

    public class Output : Statement
    {
        public string VariableName { get; private set; }

        public Output(string variableName) : base("output")
        {
            VariableName = variableName;
        }

        public override bool IsTerminal
        {
            get { return true; }
        }

        public override string ToString()
        {
            //check for variable name on the model

            if (VariableName == "this")
            {
                return Model.ToString();
            }

            var pi = Model.GetType().GetProperty(VariableName);
            if (pi != null)
            {
                return pi.GetValue(Model, null).ToString();
            }

            return "";
        }
    }
}