using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Parrot.Nodes;

namespace Parrot.Mvc.Renderers
{
    public class HtmlRenderer : IRenderer
    {

        public virtual string Render(AbstractNode node)
        {
            return Render(node, null);
        }

        public virtual string Render(AbstractNode node, object model)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            var blockNode = node as BlockNode;
            if (blockNode == null)
            {
                throw new ArgumentException("node");
            }

            var tag = CreateTag(model, blockNode);

            return tag.ToString();
        }

        public string Render(Document document, object model)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var element in document.Children)
            {
                var factory = Infrastructure.Host.DependencyResolver.Get<IRendererFactory>();

                var renderer = factory.GetRenderer(element.BlockName);
                sb.AppendLine(renderer.Render(element, model));
            }

            return sb.ToString();
        }

        protected TagBuilder CreateTag(object model, BlockNode blockNode)
        {
            object localModel = model;

            if (blockNode.Parameters != null && blockNode.Parameters.Any())
            {
                blockNode.Parameters.First().SetModel(model);

                localModel = (blockNode.Parameters.First() as ParameterNode).GetPropertyValue();
            }

            TagBuilder builder = new TagBuilder(blockNode.BlockName);
            foreach (var attribute in blockNode.Attributes.Cast<AttributeNode>())
            {
                attribute.SetModel(model);

                if (attribute.Key == "class")
                {
                    builder.AddCssClass(attribute.GetValue());
                }
                else
                {
                    builder.MergeAttribute(attribute.Key, attribute.GetValue(), true);
                }
            }

            if (blockNode.Children.Any())
            {
                if (localModel is IEnumerable && blockNode.Parameters != null && blockNode.Parameters.Any())
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (object item in localModel as IEnumerable)
                    {
                        var localItem = item;

                        foreach (var child in blockNode.Children)
                        {
                            var renderer =
                            Parrot.Infrastructure.Host.DependencyResolver.Get<IRendererFactory>().GetRenderer(child.BlockName);

                            sb.AppendLine(renderer.Render(child, localItem));
                        }
                    }
                    builder.InnerHtml += sb.ToString();
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var child in blockNode.Children)
                    {
                        if (child != null)
                        {
                            var renderer =
                            Parrot.Infrastructure.Host.DependencyResolver.Get<IRendererFactory>().GetRenderer(child.BlockName);

                            sb.Append(renderer.Render(child, localModel));
                        }
                    }

                    builder.InnerHtml = sb.ToString();
                }
            }
            else
            {
                if (blockNode.Parameters != null && blockNode.Parameters.Any())
                {
                    builder.InnerHtml = localModel != null ? localModel.ToString() : "";
                }
            }
            return builder;
        }
    }
}
