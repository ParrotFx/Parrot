namespace Parrot.Mvc.Renderers
{
    using System;
    using System.Collections.Generic;
    using Parrot.Infrastructure;
    using Parrot.Nodes;
    using Parrot.Renderers;
    using Parrot.Renderers.Infrastructure;

    public class ContentRenderer : HtmlRenderer
    {
        public ContentRenderer(IHost host) : base(host)
        {
        }

        public override IEnumerable<string> Elements
        {
            get { yield return "content"; }
        }

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