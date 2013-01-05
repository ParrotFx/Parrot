namespace Parrot.Nodes
{
    public class Attribute : AbstractNode
    {
        public string Key { get; internal set; }
        public Statement Value { get; internal set; }

        //public ValueType ValueType { get; internal set; }

        public Attribute(string key, Statement value)
        {
            Key = key;
            Value = value;
        }
    }
}