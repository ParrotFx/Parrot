using System;
using System.Linq;
using Parrot.Nodes;

namespace Parrot.Mvc.Renderers
{
    public class InputRenderer : IRenderer
    {
        public string Render(AbstractNode node, object model)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            var blockNode = node as BlockNode;

            //this tag can't have any children
            if (blockNode.Children.Any())
            {
                throw new Exception("Block can't have any children");
            }

            var localModel = model;

            TagBuilder tag = new TagBuilder("input");
            foreach (var attribute in blockNode.Attributes.Cast<AttributeNode>())
            {
                attribute.SetModel(localModel);

                if (attribute.Key == "class")
                {
                    tag.AddCssClass(attribute.GetValue());
                }
                else
                {
                    tag.MergeAttribute(attribute.Key, attribute.GetValue(), true);
                }
            }
            
            //check and see if there's a parameter and assign it to value
            if (blockNode.Parameters != null && blockNode.Parameters.Count == 1)
            {
                //grab only the first
                var parameter = blockNode.Parameters[0];
                string value = parameter.Value;
                tag.MergeAttribute("value", value, true);
            }

            return tag.ToString(TagRenderMode.SelfClosing);
        }

        public string Render(AbstractNode node)
        {
            return Render(node, null);
        }
    }
}