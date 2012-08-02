namespace Parrot.Nodes
{
    using System.Collections.Generic;

    public class ParameterNodeList : AbstractNodeList<ParameterNode>
    {
        public ParameterNodeList() : base()
        {
        }

        public ParameterNodeList(params ParameterNode[] nodes) : base()
        {
            _list.AddRange(nodes);
        }

        public ParameterNodeList(ParameterNodeList list, params ParameterNode[] nodes) : base()
        {
            _list.AddRange(list);
            _list.AddRange(nodes);
        }
    }
}