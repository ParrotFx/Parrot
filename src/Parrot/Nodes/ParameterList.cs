namespace Parrot.Nodes
{
    public class ParameterList : AbstractNodeList<Parameter>
    {
        public ParameterList() : base()
        {
        }

        public ParameterList(params Parameter[] nodes)
        {
            _list.AddRange(nodes);
        }

        public ParameterList(ParameterList list, params Parameter[] nodes)
        {
            _list.AddRange(list);
            _list.AddRange(nodes);
        }
    }
}