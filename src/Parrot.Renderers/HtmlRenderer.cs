using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Parrot.Renderers
{
    using Parrot.Infrastructure;
    using System;
    using Parrot.Renderers.Infrastructure;
    using Parrot.Nodes;

    public class HtmlRenderer : BaseRenderer, IRenderer
    {
        protected IRendererFactory RendererFactory;
        private IAttributeRenderer _attributeRenderer;

        public HtmlRenderer(IHost host, IRendererFactory rendererFactory) : base(host)
        {
            RendererFactory = rendererFactory;
            _attributeRenderer = host.DependencyResolver.Resolve<IAttributeRenderer>();
        }

        public virtual string DefaultChildTag
        {
            get { return "div"; }
        }

        public virtual void Render(StringWriter writer, Statement statement, IDictionary<string, object> documentHost, object model)
        {
            Type modelType = model != null ? model.GetType() : null;
            var modelValueProvider = ModelValueProviderFactory.Get(modelType);

            var localModel = GetLocalModelValue(documentHost, statement, modelValueProvider, model);

            CreateTag(writer, documentHost, localModel, statement, modelValueProvider);
        }

        protected virtual void CreateTag(StringWriter writer, IDictionary<string, object> documentHost, object model, Statement statement, IModelValueProvider modelValueProvider)
        {
            string tagName = string.IsNullOrWhiteSpace(statement.Name) ? DefaultChildTag : statement.Name;

            TagBuilder builder = new TagBuilder(tagName);
            //add attributes
            RenderAttributes(documentHost, model, statement, builder);
            //AppendAttributes(builder, statement.Attributes, documentHost, modelValueProvider);

            writer.Write(builder.ToString(TagRenderMode.StartTag));
            //render children

            if (statement.Children.Count > 0)
            {
                RenderChildren(writer, statement, documentHost, model);
            }

            writer.Write(builder.ToString(TagRenderMode.EndTag));
        }

        public virtual void RenderChildren(StringWriter writer, Statement statement, IDictionary<string, object> documentHost, object model, string defaultTag = null)
        {
            if (string.IsNullOrEmpty(defaultTag))
            {
                defaultTag = DefaultChildTag;
            }


            if (model is IEnumerable && statement.Parameters.Any())
            {
                foreach (object item in model as IEnumerable)
                {
                    var localItem = item;

                    RenderChildren(writer, statement.Children, documentHost, defaultTag, localItem);
                }
            }
            else
            {
                RenderChildren(writer, statement.Children, documentHost, defaultTag, model);
            }
        }

        protected void RenderChildren(StringWriter writer, StatementList children, IDictionary<string, object> documentHost, string defaultTag, object model)
        {
            Func<string, string> tagName = s => string.IsNullOrEmpty(s) ? defaultTag : s;

            foreach (var child in children)
            {
                child.Name = tagName(child.Name);
                var renderer = RendererFactory.GetRenderer(child.Name);

                renderer.Render(writer, child, documentHost, model);
            }
        }

        protected virtual void RenderAttributes(IDictionary<string, object> documentHost, object model, Statement statement, TagBuilder builder)
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
                    var renderer = RendererFactory.GetRenderer(attribute.Value.Name);

                    if (renderer is HtmlRenderer)
                    {
                        renderer = RendererFactory.GetRenderer("string");
                    }

                    StringBuilder sb = new StringBuilder();
                    var tempWriter = new StringWriter(sb);
                    renderer.Render(tempWriter, attribute.Value, documentHost, model);

                    attributeValue = sb.ToString();

                    if (_attributeRenderer != null)
                    {
                        attributeValue = _attributeRenderer.PostRender(attribute.Key, attributeValue);
                    }

                    if (attribute.Key == "class")
                    {
                        builder.AddCssClass((string)attributeValue);
                    }
                    else
                    {
                        builder.MergeAttribute(attribute.Key, (string)attributeValue, true);
                    }
                }
            }
        }
    }

    public abstract class BaseRenderer
    {
        protected IHost Host;
        protected IModelValueProviderFactory ModelValueProviderFactory;

        protected BaseRenderer(IHost host)
        {
            Host = host;
            ModelValueProviderFactory = host.DependencyResolver.Resolve<IModelValueProviderFactory>();
        }

        protected object GetLocalModelValue(IDictionary<string, object> documentHost, Statement statement, IModelValueProvider modelValueProvider, object model)
        {
            object value = null;
            if (statement.Parameters.Count > 0)
            {
                //check here first
                if (modelValueProvider.GetValue(documentHost, model, statement.Parameters[0].ValueType, statement.Parameters[0].Value, out value))
                {
                    return value;
                }
            }

            if (model != null)
            {
                //check DocumentHost next
                if (modelValueProvider.GetValue(documentHost, model, Parrot.Infrastructure.ValueType.Property, null, out value))
                {
                    return value;
                }
            }
            return model;
        }
    }
}