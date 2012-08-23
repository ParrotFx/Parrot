using System.Linq;
using System.Text;
using Parrot.Infrastructure;

namespace Parrot.Nodes
{
    public class StatementList : AbstractNodeList<Statement>
    {

        public StatementList(IHost host, params Statement[] nodes) : base(host)
        {
            _list.AddRange(nodes);
        }

        public StatementList(IHost host, StatementList list, params Statement[] nodes) : base(host)
        {
            if (list != null)
            {
                _list.AddRange(list);
            }

            if (nodes != null)
            {
                _list.AddRange(nodes);
            }
        }
    }

}