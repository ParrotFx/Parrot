using System;
using Parrot.Nodes;

namespace Parrot.Tests
{
    public class StringLiteralRenderer : IRenderer {
        public string Render(AbstractNode node, object model)
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

        public string Render(AbstractNode node)
        {
            return Render(node, null);
        }
    }
}