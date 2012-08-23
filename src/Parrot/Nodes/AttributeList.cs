using Parrot.Infrastructure;

namespace Parrot.Nodes
{
    public class AttributeList : AbstractNodeList<Attribute>
    {
        public AttributeList(IHost host, params Attribute[] nodes) : base(host)
        {
            _list.AddRange(nodes);
        }

        public AttributeList(IHost host, AttributeList list, params Attribute[] nodes) : base(host)
        {
            _list.AddRange(list);
            _list.AddRange(nodes);
        }
    }
}