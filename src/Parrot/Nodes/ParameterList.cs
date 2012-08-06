namespace Parrot.Nodes
{
    using System.Collections.Generic;

    public class ParameterList : AbstractNodeList<Parameter>
    {
        public ParameterList() : base()
        {
        }

        public ParameterList(params Parameter[] nodes) : base()
        {
            _list.AddRange(nodes);
        }

        public ParameterList(ParameterList list, params Parameter[] nodes) : base()
        {
            _list.AddRange(list);
            _list.AddRange(nodes);
        }
    }
}