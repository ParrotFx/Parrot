namespace Parrot.Renderers
{
    using Parrot.Infrastructure;
    using System;
    using System.Collections;
    using System.Linq;
    using System.Text;
    using Parrot.Renderers.Infrastructure;
    using Parrot.Nodes;

    public class HtmlRenderer : IRenderer
    {
        protected IHost Host;

        protected Func<string, object, string> PreRenderAttribute; 

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
            var factory = Host.DependencyResolver.Resolve<IRendererFactory>();

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
            var factory = Host.DependencyResolver.Resolve<IRendererFactory>();

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
            var factory = Host.DependencyResolver.Resolve<IRendererFactory>();
            
            Type modelType = model != null ? model.GetType() : null;

            object localModel = model;
            var modelValueProviderFactory = Host.DependencyResolver.Resolve<IModelValueProviderFactory>();

            if (statement.Parameters.Count > 0)
            {
                var modelValueProvider = modelValueProviderFactory.Get(modelType);

                localModel = modelValueProvider.GetValue(model, statement.Parameters[0].ValueType, statement.Parameters[0].Value);
            }

            statement.Name = string.IsNullOrEmpty(statement.Name) ? DefaultChildTag : statement.Name;

            TagBuilder builder = new TagBuilder(statement.Name);

            RenderAttributes(model, statement, builder, factory);

            if (statement.Children.Count > 0)
            {
                builder.InnerHtml = RenderChildren(statement, localModel);
            }
            return builder;
        }

        private void RenderAttributes(object model, Statement statement, TagBuilder builder, IRendererFactory factory)
        {
            foreach (var attribute in statement.Attributes)
            {
                object attributeValue = model;

                if (attribute.Value == null)
                {
                    builder.MergeAttribute(attribute.Key, attribute.Key, true);
                }
                else
                {
                    var renderer = factory.GetRenderer(attribute.Value.Name);

                    if (renderer is HtmlRenderer)
                    {
                        renderer = factory.GetRenderer("literal");
                    }

                    attributeValue = attributeValue != null 
                        ? renderer.Render(attribute.Value, model) 
                        : renderer.Render(attribute.Value);
                    
                    if (PreRenderAttribute != null)
                    {
                        attributeValue = PreRenderAttribute(attribute.Key, attributeValue);
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
            }
        }
    }
}
