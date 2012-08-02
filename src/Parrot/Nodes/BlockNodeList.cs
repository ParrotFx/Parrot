using System.Linq;
using System.Text;

namespace Parrot.Nodes
{
    public class BlockNodeList : AbstractNodeList<BlockNode>
    {

        public BlockNodeList(params BlockNode[] nodes) : base()
        {
            _list.AddRange(nodes);
        }

        public BlockNodeList(BlockNodeList list, params BlockNode[] nodes) : base()
        {
            _list.AddRange(list);
            _list.AddRange(nodes);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var element in this.Select(e => e.SetModel(Model)))
            {
                sb.Append(element.ToString());
            }

            return sb.ToString();
        }
    }

}