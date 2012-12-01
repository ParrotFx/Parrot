using Parrot.Infrastructure;

namespace Parrot.Nodes
{
    public class Attribute : AbstractNode
    {
        public string Key { get; internal set; }
        public Statement Value { get; internal set; }

        //public ValueType ValueType { get; internal set; }

        public Attribute(IHost host, string key, Statement value) : base(host)
        {
            Key = key;
            Value = value;
        }
    }
}