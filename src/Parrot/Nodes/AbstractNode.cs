using Parrot.Infrastructure;

namespace Parrot.Nodes
{
    using System.Linq;

    public abstract class AbstractNode
    {
        protected IHost Host { get; private set; }

        public AbstractNode(IHost host)
        {
            Host = host;
        }

        public abstract bool IsTerminal
        {
            get;
        }
    }
}