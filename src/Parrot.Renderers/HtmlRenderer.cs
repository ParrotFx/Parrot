using System;
using System.Collections;
using System.Linq;
using System.Text;
using Parrot.Renderers.Infrastructure;
using Parrot.Nodes;

namespace Parrot.Renderers
{
    using Parrot.Infrastructure;
    using Parrot.Renderers;
    using Attribute = Nodes.Attribute;

    public class HtmlRenderer : IRenderer
    {
        protected IHost Host;

        public HtmlRenderer(IHost host)
        {
            Host = host;
        }
        
        public virtual string DefaultChildTag
        {
            get { return "div"; }
        }

        public virtual string RenderChildren(Statement statement, object localModel, string defaultTag = null)
        {
            var factory = Host.DependencyResolver.Get<IRendererFactory>();

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
                        var renderer = factory.GetRenderer(child.Name);

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
                        var renderer = factory.GetRenderer(child.Name);

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
            var factory = Host.DependencyResolver.Get<IRendererFactory>();

            StringBuilder sb = new StringBuilder();
            foreach (var element in document.Children)
            {
                var renderer = factory.GetRenderer(element.Name);
                sb.AppendLine(renderer.Render(element, model));
            }

            return sb.ToString();
        }

        protected TagBuilder CreateTag(object model, Statement statement)
        {
            Type modelType = model != null ? model.GetType() : null;

            object localModel = model;
            var modelValueProviderFactory = Host.DependencyResolver.Get<IModelValueProviderFactory>();
            
            if (statement.Parameters.Count > 0)
            {
                var modelValueProvider = modelValueProviderFactory.Get(modelType);

                localModel = modelValueProvider.GetValue(model, statement.Parameters[0].ValueType, statement.Parameters[0].Value);
            }

            statement.Name = string.IsNullOrEmpty(statement.Name) ? DefaultChildTag : statement.Name;

            TagBuilder builder = new TagBuilder(statement.Name);
            foreach (var attribute in statement.Attributes)
            {
                object attributeValue = model;
                if (attributeValue != null)
                {
                    attributeValue = modelValueProviderFactory.Get(modelType).GetValue(model, attribute.ValueType, attribute.Value);
                }
                else
                {
                    attributeValue = modelValueProviderFactory.Get(typeof(object)).GetValue(model, attribute.ValueType, attribute.Value);
                }

                if (attribute.Key == "class")
                {
                    builder.AddCssClass((string) attributeValue);
                }
                else
                {
                    builder.MergeAttribute(attribute.Key, (string) attributeValue, true);
                }
            }

            if (statement.Children.Count > 0)
            {
                builder.InnerHtml = RenderChildren(statement, localModel);
            }
            else
            {
                if (statement.Parameters.Count > 0)
                {
                    builder.InnerHtml = localModel != null ? localModel.ToString() : "";
                }
            }
            return builder;
        }
    }
}
