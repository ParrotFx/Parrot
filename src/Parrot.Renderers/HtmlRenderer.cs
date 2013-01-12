﻿namespace Parrot.Renderers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Parrot.Infrastructure;
    using Parrot.Nodes;
    using Parrot.Renderers.Infrastructure;

    public class HtmlRenderer : BaseRenderer, IRenderer
    {
        public HtmlRenderer(IHost host)
        {
            Host = host;
        }

        public virtual string DefaultChildTag
        {
            get { return "div"; }
        }

        protected override IHost Host { get; set; }

        public virtual IEnumerable<string> Elements
        {
            get { yield return "*"; }
        }

        public virtual void Render(IParrotWriter writer, IRendererFactory rendererFactory, Statement statement, IDictionary<string, object> documentHost, object model)
        {
            Type modelType = model != null ? model.GetType() : null;
            var modelValueProvider = Host.ModelValueProviderFactory.Get(modelType);

            var localModel = GetLocalModelValue(documentHost, statement, modelValueProvider, model);

            CreateTag(writer, rendererFactory, documentHost, localModel, statement, modelValueProvider);
        }

        protected virtual void CreateTag(IParrotWriter writer, IRendererFactory rendererFactory, IDictionary<string, object> documentHost, object model, Statement statement, IModelValueProvider modelValueProvider)
        {
            string tagName = string.IsNullOrWhiteSpace(statement.Name) ? DefaultChildTag : statement.Name;

            TagBuilder builder = new TagBuilder(tagName);
            //add attributes
            RenderAttributes(rendererFactory, documentHost, model, statement, builder);
            //AppendAttributes(builder, statement.Attributes, documentHost, modelValueProvider);

            writer.Write(builder.ToString(TagRenderMode.StartTag));
            //render children

            if (statement.Children.Count > 0)
            {
                RenderChildren(writer, statement, rendererFactory, documentHost, model);
            }

            writer.Write(builder.ToString(TagRenderMode.EndTag));
        }

        public virtual void RenderChildren(IParrotWriter writer, Statement statement, IRendererFactory rendererFactory, IDictionary<string, object> documentHost, object model, string defaultTag = null)
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

                    RenderChildren(writer, statement.Children, rendererFactory, documentHost, defaultTag, localItem);
                }
            }
            else
            {
                RenderChildren(writer, statement.Children, rendererFactory, documentHost, defaultTag, model);
            }
        }
        
        private IList<object> ToList(IEnumerable loop)
        {
            var list = new List<object>();
            foreach (var item in loop)
            {
                list.Add(item);
            }

            return list;
        }

        protected void RenderChildren(IParrotWriter writer, StatementList children, IRendererFactory rendererFactory, IDictionary<string, object> documentHost, string defaultTag, object model)
        {
            Func<string, string> tagName = s => string.IsNullOrEmpty(s) ? defaultTag : s;

            foreach (var child in children)
            {
                child.Name = tagName(child.Name);
                var renderer = rendererFactory.GetRenderer(child.Name);

                renderer.Render(writer, rendererFactory, child, documentHost, model);
            }
        }

        protected virtual string RenderAttribute(Nodes.Attribute attribute, IRendererFactory rendererFactory, IDictionary<string, object> documentHost, object model)
        {
            var renderer = rendererFactory.GetRenderer(attribute.Value.Name);

            if (renderer is HtmlRenderer)
            {
                renderer = rendererFactory.GetRenderer("string");
            }

            //render attribute
            var tempWriter = Host.CreateWriter();
            renderer.Render(tempWriter, rendererFactory, attribute.Value, documentHost, model);

            var attributeValue = tempWriter.Result();

            if (Host.PathResolver != null)
            {
                attributeValue = Host.PathResolver.ResolveAttributeRelativePath(attribute.Key, attributeValue);
            }

            return attributeValue;
        }

        protected virtual void RenderAttributes(IRendererFactory rendererFactory, IDictionary<string, object> documentHost, object model, Statement statement, TagBuilder builder)
        {
            foreach (var attribute in statement.Attributes)
            {
                if (attribute.Value == null)
                {
                    builder.MergeAttribute(attribute.Key, attribute.Key, true);
                }
                else
                {
                    object attributeValue = RenderAttribute(attribute, rendererFactory, documentHost, model);

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

    public abstract class BaseRenderer
    {
        protected abstract IHost Host { get; set; }

        protected object GetLocalModelValue(IDictionary<string, object> documentHost, Statement statement, IModelValueProvider modelValueProvider, object model)
        {
            object value;
            if (statement.Parameters.Count > 0)
            {
                //check here first
                if (modelValueProvider.GetValue(documentHost, model, statement.Parameters[0].Value, out value))
                {
                    return value;
                }
            }

            if (model != null)
            {
                //check DocumentHost next
                //if (modelValueProvider.GetValue(documentHost, model, null, out value))
                if (modelValueProvider.GetValue(documentHost, model, Parrot.Infrastructure.ValueType.Property, null, out value))
                {
                    return value;
                }
            }
            return model;
        }

    }
}