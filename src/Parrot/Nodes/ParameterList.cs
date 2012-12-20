using Parrot.Infrastructure;

namespace Parrot.Nodes
{
    public class ParameterList : AbstractNodeList<Parameter>
    {
        public ParameterList(IHost host) : base(host)
        {
        }

        public ParameterList(IHost host, params Parameter[] nodes) : base(host)
        {
            List.AddRange(nodes);
        }

        public ParameterList(IHost host, ParameterList list, params Parameter[] nodes)  : base(host)
        {
            List.AddRange(list);
            List.AddRange(nodes);
        }
    }
}