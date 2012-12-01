using Parrot.Infrastructure;

namespace Parrot.Nodes
{
    using System.Linq;

    public abstract class AbstractNode
    {
        protected IHost Host { get; private set; }

        protected AbstractNode(IHost host)
        {
            Host = host;
        }
    }
}