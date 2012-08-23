using Parrot.Infrastructure;

namespace Parrot.Nodes
{
    public class Parameter : AbstractNode
    {
        public string Value { get; private set; }

        public ValueType ValueType { get; private set; }

        public Parameter(IHost host, string value) : base(host)
        {
            var valueTypeProvider = host.DependencyResolver.Get<IValueTypeProvider>();
            var result = valueTypeProvider.GetValue(value);

            ValueType = result.Type;
            Value = result.Value as string;
        }

        public override bool IsTerminal
        {
            get { return true; }
        }
    }
}