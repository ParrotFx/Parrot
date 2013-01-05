namespace Parrot.Nodes
{
    public class StatementTail : AbstractNode
    {
        public ParameterList Parameters { get; set; }
        public AttributeList Attributes { get; set; }
        public StatementList Children { get; set; }
    }
}