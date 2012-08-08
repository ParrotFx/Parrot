using System;
using Parrot.Infrastructure;
using Parrot.Nodes;

namespace Parrot.Mvc.Renderers
{
    public class StringLiteralRenderer : IRenderer
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

            var stringNode = node as StringLiteral;
            if (stringNode == null)
            {
                throw new InvalidCastException("node");
            }

            return stringNode.GetValue() as string;
        }
    }
}