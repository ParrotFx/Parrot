using System;
using System.Collections;
using System.Linq;
using System.Text;
using Parrot.Nodes;

namespace Parrot.Mvc.Renderers
{
    using Attribute = Nodes.Attribute;

    public class HtmlRenderer : IRenderer
    {
        protected Lazy<IRendererFactory> Factory = new Lazy<IRendererFactory>(() => Infrastructure.Host.DependencyResolver.Get<IRendererFactory>()); 
        
        public virtual string DefaultChildTag
        {
            get { return "div"; }
        }

        public virtual string RenderChildren(Statement statement, object localModel, string defaultTag = null)
        {
            if (string.IsNullOrEmpty(defaultTag))
            {
                defaultTag = DefaultChildTag;
            }

            Func<string, string> tagName = s => string.IsNullOrEmpty(s) ? defaultTag : s;

            var sb = new StringBuilder();
            if (localModel is IEnumerable && statement.Parameters != null && statement.Parameters.Any())
            {
                foreach (object item in localModel as IEnumerable)
                {
                    var localItem = item;

                    foreach (var child in statement.Children)
                    {
                        child.Name = tagName(child.Name);
                        var renderer = Factory.Value.GetRenderer(child.Name);

                        sb.AppendLine(renderer.Render(child, localItem));
                    }
                }
            }
            else
            {
                foreach (var child in statement.Children)
                {
                    if (child != null)
                    {
                        child.Name = tagName(child.Name);
                        var renderer = Factory.Value.GetRenderer(child.Name);

                        sb.Append(renderer.Render(child, localModel));
                    }
                }
            }

            return sb.ToString();
        }

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

            var blockNode = node as Statement;
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

                var renderer = factory.GetRenderer(element.Name);
                sb.AppendLine(renderer.Render(element, model));
            }

            return sb.ToString();
        }

        protected TagBuilder CreateTag(object model, Statement statement)
        {
            object localModel = model;

            if (statement.Parameters != null && statement.Parameters.Any())
            {
                //statement.Parameters.First().SetModel(model);

                localModel = RendererHelpers.GetModelValue(model, statement.Parameters.First().ValueType, statement.Parameters.First().Value);
            }

            statement.Name = string.IsNullOrEmpty(statement.Name) ? DefaultChildTag : statement.Name;

            TagBuilder builder = new TagBuilder(statement.Name);
            foreach (var attribute in statement.Attributes)
            {
                var attributeValue = RendererHelpers.GetModelValue(model, attribute.ValueType, attribute.Value);

                if (attribute.Key == "class")
                {
                    builder.AddCssClass((string) attributeValue);
                }
                else
                {
                    builder.MergeAttribute(attribute.Key, (string) attributeValue, true);
                }
            }

            if (statement.Children.Any())
            {
                builder.InnerHtml = RenderChildren(statement, localModel);
            }
            else
            {
                if (statement.Parameters != null && statement.Parameters.Any())
                {
                    builder.InnerHtml = localModel != null ? localModel.ToString() : "";
                }
            }
            return builder;
        }
    }
}
