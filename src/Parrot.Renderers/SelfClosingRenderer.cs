using System;
using System.Collections.Generic;
using Parrot.Renderers.Infrastructure;
using Parrot.Nodes;

namespace Parrot.Renderers
{
    using Parrot.Infrastructure;

    public class SelfClosingRenderer : HtmlRenderer
    {
        public SelfClosingRenderer(IHost host) : base(host) { }

        public override IEnumerable<string> Elements
        {
            get { return new[] {"base", "basefont", "frame", "link", "meta", "area", "br", "col", "hr", "img", "param"}; }
        }

        public override void Render(IParrotWriter writer, IRendererFactory rendererFactory, Statement statement, IDictionary<string, object> documentHost, object model)
        {
            Type modelType = model != null ? model.GetType() : null;
            var modelValueProvider = ModelValueProviderFactory.Get(modelType);

            var localModel = GetLocalModelValue(documentHost, statement, modelValueProvider, model);

            CreateTag(writer, rendererFactory, documentHost, localModel, statement, modelValueProvider);
        }

        protected override void CreateTag(IParrotWriter writer, IRendererFactory rendererFactory, IDictionary<string, object> documentHost, object model, Statement statement, IModelValueProvider modelValueProvider)
        {
            string tagName = string.IsNullOrWhiteSpace(statement.Name) ? DefaultChildTag : statement.Name;

            TagBuilder builder = new TagBuilder(tagName);
            //add attributes
            RenderAttributes(rendererFactory, documentHost, model, statement, builder);

            writer.Write(builder.ToString(TagRenderMode.SelfClosing));
        }

    }
}