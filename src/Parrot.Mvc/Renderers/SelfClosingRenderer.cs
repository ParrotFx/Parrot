using System;
using System.Collections.Generic;
using Parrot.Infrastructure;
using Parrot.Nodes;

namespace Parrot.Mvc.Renderers
{
    public class SelfClosingRenderer : HtmlRenderer
    {
        public override string Render(AbstractNode node, object model, LocalsStack stack)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            var blockNode = node as Statement;
            if (blockNode == null)
            {
                throw new ArgumentException("node");
            }

            var tag = CreateTag(model, blockNode, stack);

            return tag.ToString(TagRenderMode.SelfClosing);
        }
    }
}