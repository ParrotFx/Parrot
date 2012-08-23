using System;
using Parrot.Renderers.Infrastructure;
using Parrot.Nodes;

namespace Parrot.Renderers
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

            var value = RendererHelpers.GetModelValue(model, Parrot.Infrastructure.ValueType.Property, outputNode.VariableName);

            //html encode this!
            return System.Net.WebUtility.HtmlEncode(value.ToString());
        }

        public string Render(AbstractNode node)
        {
            return Render(node, null);
        }
    }
}