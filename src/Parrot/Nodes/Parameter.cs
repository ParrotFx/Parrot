namespace Parrot.Nodes
{
    public class Parameter : AbstractNode
    {
        public string Value { get; private set; }

        public Parameter(string value)
        {
            Value = value;
        }
    }
}