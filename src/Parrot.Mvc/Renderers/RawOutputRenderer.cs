using System;
using Parrot.Infrastructure;
using Parrot.Nodes;
using ValueType = Parrot.Infrastructure.ValueType;

namespace Parrot.Mvc.Renderers
{
    public class RawOutputRenderer : IRenderer
    {
        public string Render(AbstractNode node, object model)
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

            var value = RendererHelpers.GetModelValue(model, ValueType.Property, outputNode.VariableName);

            return value.ToString();
        }

        public string Render(AbstractNode node)
        {
            return Render(node, null);
        }
    }
}