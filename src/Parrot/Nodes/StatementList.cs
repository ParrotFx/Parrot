using System.Linq;
using System.Text;

namespace Parrot.Nodes
{
    public class StatementList : AbstractNodeList<Statement>
    {

        public StatementList(params Statement[] nodes)
        {
            _list.AddRange(nodes);
        }

        public StatementList(StatementList list, params Statement[] nodes)
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