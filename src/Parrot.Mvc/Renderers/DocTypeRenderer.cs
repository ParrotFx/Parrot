using System;
using System.Linq;
using Parrot.Nodes;

namespace Parrot.Mvc.Renderers
{
    public class DocTypeRenderer : IRenderer
    {
        public string Render(AbstractNode node, object model)
        {
            //assert we have a blocknode
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            var docTypeNode = node as BlockNode;
            if (docTypeNode == null || !docTypeNode.BlockName.Equals("doctype", StringComparison.OrdinalIgnoreCase))
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

        public string Render(AbstractNode node)
        {
            return Render(node, null);
        }
    }
}