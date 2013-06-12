namespace Parrot.Renderers
{
    using System;
    using System.Collections.Generic;
    using Parrot.Infrastructure;
    using Parrot.Nodes;
    using Parrot.Renderers.Infrastructure;

    public class SelfClosingRenderer : HtmlRenderer
    {
        public SelfClosingRenderer(IHost host) : base(host)
        {
        }

        public override IEnumerable<string> Elements
        {
            get { return new[] {"base", "basefont", "frame", "link", "meta", "area", "br", "col", "hr", "img", "param"}; }
        }

        public override void Render(IParrotWriter writer, IRendererFactory rendererFactory, Statement statement, IDictionary<string, object> documentHost, object model)
        {
            var localModel = GetLocalModel(documentHost, statement, model);
            CreateTag(writer, rendererFactory, documentHost, localModel, statement);
        }

        protected override void CreateTag(IParrotWriter writer, IRendererFactory rendererFactory, IDictionary<string, object> documentHost, object model, Statement statement)
        {
            string tagName = string.IsNullOrWhiteSpace(statement.Name) ? DefaultChildTag : statement.Name;

            TagBuilder builder = new TagBuilder(tagName);
            //add attributes
            RenderAttributes(rendererFactory, documentHost, model, statement, builder);

            writer.Write(builder.ToString(TagRenderMode.SelfClosing));
        }
    }
}