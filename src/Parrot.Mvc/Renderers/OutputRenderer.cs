using System;
using Parrot.Nodes;

namespace Parrot.Mvc.Renderers
{
    public class OutputRenderer : IRenderer
    {
        public string Render(AbstractNode node, object model)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            var outputNode = node as EncodedOutput;
            if (outputNode == null)
            {
                throw new ArgumentNullException("node");
            }

            node.SetModel(model);
            return node.ToString();
        }

        public string Render(AbstractNode node)
        {
            return Render(node, null);
        }
    }
}
