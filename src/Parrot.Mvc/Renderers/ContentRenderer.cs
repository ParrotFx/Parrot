using System.Collections.Generic;
using System.IO;

namespace Parrot.Mvc.Renderers
{
    using Parrot.Infrastructure;
    using System;
    using Parrot.Nodes;
    using Parrot.Renderers;
    using Parrot.Renderers.Infrastructure;

    public class ContentRenderer : HtmlRenderer
    {
        public ContentRenderer(IHost host) : base(host) { }

        public override void Render(IParrotWriter writer, IRendererFactory rendererFactory, Statement statement, IDictionary<string, object> documentHost, object model)
        {
            var childrenQueue = documentHost.GetValueOrDefault("_LayoutChildren_") as Queue<StatementList>;
            if (childrenQueue == null)
            {
                //TODO: replace this with a real exception
                throw new Exception("Children elements empty");
            }

            var children = childrenQueue.Dequeue();

            RenderChildren(writer, children, rendererFactory, documentHost, DefaultChildTag, model);
        }
    }
}