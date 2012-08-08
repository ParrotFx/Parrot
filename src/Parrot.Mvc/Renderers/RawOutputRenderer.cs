using System;
using Parrot.Infrastructure;
using Parrot.Nodes;

namespace Parrot.Mvc.Renderers
{
    public class RawOutputRenderer : IRenderer
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

            var outputNode = node as RawOutput;
            if (outputNode == null)
            {
                throw new ArgumentNullException("node");
            }

            node.SetModel(model);
            node.SetStack(stack);
            return node.ToString();
        }
    }
}