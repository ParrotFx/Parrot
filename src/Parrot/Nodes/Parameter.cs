namespace Parrot.Nodes
{
    public class Parameter : AbstractNode
    {
        public string Value { get; private set; }
        public string CalculatedValue { get; set; }

        public Parameter(string value)
        {
            Value = value;
            CalculatedValue = value;
        }
    }
}