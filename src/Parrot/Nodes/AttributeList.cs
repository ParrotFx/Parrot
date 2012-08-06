namespace Parrot.Nodes
{
    using Parrot.Nodes;

    public class AttributeList : AbstractNodeList<Attribute>
    {
        public AttributeList(params Attribute[] nodes) : base()
        {
            _list.AddRange(nodes);
        }

        public AttributeList(AttributeList list, params Attribute[] nodes) : base()
        {
            _list.AddRange(list);
            _list.AddRange(nodes);
        }
    }
}