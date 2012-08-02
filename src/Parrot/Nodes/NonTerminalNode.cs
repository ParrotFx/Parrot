namespace Parrot.Nodes
{
    using System.Collections;
    using GoldParser;
    using Parrot.Nodes;

    /// <summary>
    /// Derive this class for NonTerminal AST Nodes
    /// </summary>
    public class NonTerminalNode : AbstractNode
    {
        private int m_reductionNumber;
        private Rule m_rule;
        private ArrayList m_array = new ArrayList();

        public NonTerminalNode(Parser theParser)
        {
            m_rule = theParser.ReductionRule;
        }

        public override bool IsTerminal
        {
            get
            {
                return false;
            }
        }

        public int ReductionNumber
        {
            get { return m_reductionNumber; }
            set { m_reductionNumber = value; }
        }

        public int Count
        {
            get { return m_array.Count; }
        }

        public AbstractNode this[int index]
        {
            get { return m_array[index] as AbstractNode; }
        }

        public void AppendChildNode(AbstractNode node)
        {
            if (node == null)
            {
                return;
            }
            m_array.Add(node);
        }

        public Rule Rule
        {
            get { return m_rule; }
        }

    }
}