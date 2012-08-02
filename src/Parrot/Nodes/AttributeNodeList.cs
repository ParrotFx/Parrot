namespace Parrot.Nodes
{
    using Parrot.Nodes;

    public class AttributeNodeList : AbstractNodeList<AttributeNode>
    {
        public AttributeNodeList(params AttributeNode[] nodes) : base()
        {
            _list.AddRange(nodes);
        }

        public AttributeNodeList(AttributeNodeList list, params AttributeNode[] nodes) : base()
        {
            _list.AddRange(list);
            _list.AddRange(nodes);
        }
    }
}