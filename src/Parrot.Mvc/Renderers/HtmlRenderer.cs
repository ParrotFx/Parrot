using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Parrot.Infrastructure;
using Parrot.Nodes;

namespace Parrot.Mvc.Renderers
{
    public class HtmlRenderer : IRenderer
    {
        protected Lazy<IRendererFactory> Factory = new Lazy<IRendererFactory>(() => Host.DependencyResolver.Get<IRendererFactory>());

        public HtmlRenderer()
        {
        }

        public virtual string DefaultChildTag
        {
            get { return "div"; }
        }

        public virtual string RenderChildren(Statement statement, object localModel, LocalsStack stack, string defaultTag = null)
        {
            if (string.IsNullOrEmpty(defaultTag))
            {
                defaultTag = DefaultChildTag;
            }

            Func<string, string> tagName = (s) => string.IsNullOrEmpty(s) ? defaultTag : s;

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

                        sb.AppendLine(renderer.Render(child, localItem, stack));
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

                        sb.Append(renderer.Render(child, localModel, stack));
                    }
                }
            }

            return sb.ToString();
        }

        public string Render(Document document, object model, LocalsStack stack)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var element in document.Children)
            {
                var renderer = Factory.Value.GetRenderer(element.Name);
                sb.AppendLine(renderer.Render(element, model, stack));
            }

            return sb.ToString();
        }

        protected TagBuilder CreateTag(object model, Statement statement, LocalsStack stack)
        {
            object localModel = model;

            if (statement.Parameters != null && statement.Parameters.Any())
            {
                statement.Parameters.First().SetModel(model);
                statement.Parameters[0].SetStack(stack);

                localModel = statement.Parameters.First().GetPropertyValue();
            }

            statement.Name = string.IsNullOrEmpty(statement.Name) ? DefaultChildTag : statement.Name;

            TagBuilder builder = new TagBuilder(statement.Name);
            foreach (var attribute in statement.Attributes)
            {
                attribute.SetModel(model);
                attribute.SetStack(stack);

                if (attribute.Key == "class")
                {
                    builder.AddCssClass(attribute.GetValue());
                }
                else
                {
                    builder.MergeAttribute(attribute.Key, attribute.GetValue(), true);
                }
            }

            if (statement.Children.Any())
            {
                builder.InnerHtml = RenderChildren(statement, localModel, stack);
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

        public virtual string Render(AbstractNode node, LocalsStack stack)
        {
            return Render(node, null, stack);
        }

        public virtual string Render(AbstractNode node, object model, LocalsStack stack)
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

            return tag.ToString();
        }
    }
}
