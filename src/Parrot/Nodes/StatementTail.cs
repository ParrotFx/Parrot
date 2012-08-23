using Parrot.Infrastructure;

namespace Parrot.Nodes
{
    public class StatementTail : AbstractNode
    {

        public StatementTail(IHost host) : base(host) {}
        public override bool IsTerminal
        {
            get { return false; }
        }

        public ParameterList Parameters { get; set; }
        public AttributeList Attributes { get; set; }
        public StatementList Children { get; set; }
    }
}