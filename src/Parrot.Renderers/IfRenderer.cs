namespace Parrot.Renderers
{
    using System;
    using System.Collections.Generic;
    using Parrot.Renderers.Infrastructure;

    public class IfRenderer : HtmlRenderer
    {
        public IfRenderer(IHost host) : base(host) { }

        public override IEnumerable<string> Elements
        {
            get { yield return "if"; }
        }

        public override void Render(Parrot.Infrastructure.IParrotWriter writer, IRendererFactory rendererFactory, Nodes.Statement statement, IDictionary<string, object> documentHost, object model)
        {
            var localModel = GetLocalModel(documentHost, statement, model);

            if (localModel is bool && (bool)localModel)
            {
                RenderChildren(writer, statement.Children, rendererFactory, documentHost, base.DefaultChildTag, model);
            }
        }
    }
}