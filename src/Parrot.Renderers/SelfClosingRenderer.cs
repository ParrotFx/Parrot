using System;
using System.Collections.Generic;
using Parrot.Renderers.Infrastructure;
using Parrot.Nodes;

namespace Parrot.Renderers
{
    using Parrot.Infrastructure;

    public class SelfClosingRenderer : HtmlRenderer
    {
        public SelfClosingRenderer(IHost host, IRendererFactory rendererFactory) : base(host, rendererFactory) { }

        public override void Render(IParrotWriter writer, Statement statement, IDictionary<string, object> documentHost, object model)
        {
            Type modelType = model != null ? model.GetType() : null;
            var modelValueProvider = ModelValueProviderFactory.Get(modelType);

            var localModel = GetLocalModelValue(documentHost, statement, modelValueProvider, model);

            CreateTag(writer, documentHost, localModel, statement, modelValueProvider);
        }

        protected override void CreateTag(IParrotWriter writer, IDictionary<string, object> documentHost, object model, Statement statement, IModelValueProvider modelValueProvider)
        {
            string tagName = string.IsNullOrWhiteSpace(statement.Name) ? DefaultChildTag : statement.Name;

            TagBuilder builder = new TagBuilder(tagName);
            //add attributes
            RenderAttributes(documentHost, model, statement, builder);

            writer.Write(builder.ToString(TagRenderMode.SelfClosing));
        }

    }
}