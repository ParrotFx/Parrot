using System;
using System.Linq;
using Parrot.Infrastructure;
using Parrot.Nodes;

namespace Parrot.Mvc.Renderers
{
    public class DocTypeRenderer : IRenderer
    {
        public string Render(AbstractNode node, LocalsStack stack)
        {
            return Render(node, null, stack);
        }

        public string Render(AbstractNode node, object model, LocalsStack stack)
        {
            //assert we have a blocknode
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            var docTypeNode = node as Statement;
            if (docTypeNode == null || !docTypeNode.Name.Equals("doctype", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidCastException("node was not a docType element");
            }

            var value = "html"; //default value of "html"

            var parameter = docTypeNode.Parameters != null ? docTypeNode.Parameters.FirstOrDefault() : null;
            if (parameter != null)
            {
                value = parameter.Value;
            }

            return string.Format("<!DOCTYPE {0}>", value);
        }
    }
}