namespace Parrot.Nodes
{
    using System.Collections;
    using Parrot.Nodes;
    using GOLD;

    /// <summary>
    /// Derive this class for NonTerminal AST Nodes
    /// </summary>
    public class NonTerminalNode : AbstractNode
    {
        private int m_reductionNumber;
        private ArrayList m_array = new ArrayList();

        public NonTerminalNode(Parser theParser)
        {
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

    }
}