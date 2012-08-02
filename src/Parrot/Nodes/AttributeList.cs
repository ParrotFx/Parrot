namespace Parrot.Nodes
{
    using Parrot.Nodes;

    public class AttributeList : AbstractNodeList
    {

        public AttributeList(AttributeNode attributeNode, string key, string value)
        {
            _list.Add(attributeNode);
            _list.Add(new AttributeNode(key, value));
        }

    }
}