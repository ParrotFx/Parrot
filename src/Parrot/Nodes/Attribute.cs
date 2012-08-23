using Parrot.Infrastructure;

namespace Parrot.Nodes
{
    public class Attribute : AbstractNode
    {
        public string Key { get; internal set; }
        public string Value { get; internal set; }

        public ValueType ValueType { get; internal set; }

        public Attribute(IHost host, string key, string value) : base(host)
        {
            Key = key;

            var valueTypeProvider = host.DependencyResolver.Get<IValueTypeProvider>();
            var result = valueTypeProvider.GetValue(value);

            ValueType = result.Type;
            Value = result.Value as string;
        }
        
        public override bool IsTerminal
        {
            get { return false; }
        }

        public override string ToString()
        {

            if (ValueType == ValueType.Property)
            {
                return string.Format("{0}=\"{1}\"", Key, Value);
            }

            return string.Format("{0}={1}", Key, Value);
        }
    }
}