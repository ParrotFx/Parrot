namespace Parrot.Nodes
{
    public class AttributeList : AbstractNodeList<Attribute>
    {
        public AttributeList(params Attribute[] nodes)
        {
            List.AddRange(nodes);
        }

        public AttributeList(AttributeList list, params Attribute[] nodes)
        {
            List.AddRange(list);
            List.AddRange(nodes);
        }
    }
}