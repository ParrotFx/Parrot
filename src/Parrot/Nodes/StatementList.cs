using System.Collections.Generic;
using System.Linq;
using System.Text;
using Parrot.Infrastructure;

namespace Parrot.Nodes
{
    public class StatementList : AbstractNodeList<Statement>
    {
        public StatementList(IHost host, params Statement[] nodes) : base(host)
        {
            List.AddRange(nodes);
        }

        public StatementList(IHost host, StatementList list, params Statement[] nodes) : base(host)
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