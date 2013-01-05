namespace Parrot.Nodes
{
    public class ParameterList : AbstractNodeList<Parameter>
    {
        public ParameterList()
        {
        }

        public ParameterList(params Parameter[] nodes)
        {
            List.AddRange(nodes);
        }

        public ParameterList(ParameterList list, params Parameter[] nodes)
        {
            List.AddRange(list);
            List.AddRange(nodes);
        }
    }
}