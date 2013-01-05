namespace Parrot.Nodes
{
    public class StatementList : AbstractNodeList<Statement>
    {
        public StatementList(params Statement[] nodes)
        {
            List.AddRange(nodes);
        }

        public StatementList(StatementList list, params Statement[] nodes)
        {
            if (list != null)
            {
                List.AddRange(list);
            }

            if (nodes != null)
            {
                List.AddRange(nodes);
            }
        }
    }
}