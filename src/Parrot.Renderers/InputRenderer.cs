using System;
using System.Linq;
using Parrot.Renderers.Infrastructure;
using Parrot.Nodes;

namespace Parrot.Renderers
{
    using Parrot.Infrastructure;
    using Parrot.Renderers;

    public class InputRenderer : IRenderer
    {
        private readonly IHost _host;

        public InputRenderer(IHost host)
        {
            _host = host;
        }

        public string Render(AbstractNode node, object documentHost)
        {
            var factory = _host.DependencyResolver.Resolve<IRendererFactory>();
            var modelValueProviderFactory = _host.DependencyResolver.Resolve<IModelValueProviderFactory>();
            
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

            var localModel = documentHost;

            TagBuilder tag = new TagBuilder("input");
            foreach (var attribute in blockNode.Attributes)
            {
                object attributeValue = documentHost;

                if (attribute.Value != null)
                {
                    var renderer = factory.GetRenderer(attribute.Value.Name);

                    if (renderer is HtmlRenderer)
                    {
                        //nope!
                        renderer = factory.GetRenderer("literal");
                    }
                    //attributeValue = modelValueProviderFactory.Get(modelType).GetValue(model, attribute.ValueType, attribute.Value);
                    attributeValue = renderer.Render(attribute.Value, documentHost);
                }
                else
                {
                    //var renderer = factory.GetRenderer(attribute.Value.Name);
                    //attributeValue = modelValueProviderFactory.Get(typeof(object)).GetValue(model, attribute.ValueType, attribute.Value);
                    attributeValue = null;
                }
                if (attribute.Key == "class")
                {
                    tag.AddCssClass((string)attributeValue);
                }
                else
                {
                    //attributeValue = modelValueProviderFactory.Get(typeof (object)).GetValue(attributeValue);

                    if (attributeValue == null || attributeValue.Equals("true"))
                    {
                        tag.MergeAttribute(attribute.Key, attribute.Key, true);
                    }
                    else if (attributeValue == null || attributeValue.Equals("false") || attributeValue.Equals("null"))
                    {
                        //no output
                    }
                    //else if (noOutput(attributeValue))
                    //{
                    //    //checked=false should not output the checked attribute
                    //    //checked=null should not output the checked attribute
                    //}
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