using System;
using Parrot.Infrastructure;
using Parrot.Nodes;

namespace Parrot.Mvc.Renderers
{
    public class OutputRenderer : IRenderer
    {
        public string Render(AbstractNode node, LocalsStack stack)
        {
            return Render(node, null, stack);
        }

        public string Render(AbstractNode node, object model, LocalsStack stack)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            var outputNode = node as Output;
            if (outputNode == null)
            {
                throw new ArgumentNullException("node");
            }

            node.SetModel(model);
            return node.ToString();
        }
    }
}
