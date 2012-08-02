namespace Parrot.Nodes
{
    using Parrot.Nodes;

    public class ClassIdentifierNode : AbstractNode
    {
        public string ClassId { get; private set; }

        public ClassIdentifierNode(string token)
        {
            ClassId = token.Substring(1);
        }

        public override bool IsTerminal
        {
            get { return true; }
        }
    }
}