using System;
using System.Linq;
using Parrot.Infrastructure;
using Parrot.Nodes;
using ValueType = Parrot.Infrastructure.ValueType;

namespace Parrot.Renderers
{
    using Parrot.Renderers;

    public class InputRenderer : IRenderer
    {
        public string Render(AbstractNode node, object model)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            var blockNode = node as Statement;

            //this tag can't have any children
            if (blockNode.Children.Any())
            {
                throw new Exception("Block can't have any children");
            }

            var localModel = model;

            TagBuilder tag = new TagBuilder("input");
            foreach (var attribute in blockNode.Attributes)
            {
                //attribute.SetModel(localModel);
                var attributeValue = model == null && attribute.ValueType == ValueType.Property 
                    ? null 
                    : RendererHelpers.GetModelValue(model, attribute.ValueType, attribute.Value);

                if (attribute.Key == "class")
                {
                    tag.AddCssClass((string)attributeValue);
                }
                else
                {

                    Func<object, ValueType, bool> noOutput = (a, v) =>
                    {
                        if (a is bool && !(bool) attributeValue)
                        {
                            return true;
                        }

                        if (attributeValue == null && v == ValueType.Keyword)
                        {
                            return true;
                        }

                        return false;
                    };

                    if (attributeValue is bool && (bool)attributeValue)
                    {
                        tag.MergeAttribute(attribute.Key, attribute.Key, true);
                    }
                    else if (noOutput(attributeValue, attribute.ValueType))
                    {
                        //checked=false should not output the checked attribute
                        //checked=null should not output the checked attribute
                    }
                    else
                    {
                        tag.MergeAttribute(attribute.Key, (string)attributeValue, true);
                    }
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