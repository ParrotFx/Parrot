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


        //public string RenderFromLayout(AbstractNode node, StatementList children, object model)
        //{
        //    Document document = new Document(_host)
        //    {
        //        Children = children
        //    };

        //    return _host.DependencyResolver.Resolve<DocumentRenderer>().Render(document, model);
        //}

        //public string Render(AbstractNode node, object documentHost)
        //{
        //    dynamic localModel = documentHost;

        //    Document document = new Document(_host)
        //    {
        //        Children = localModel.Children
        //    };

        //    return _host.DependencyResolver.Resolve<DocumentRenderer>().Render(document, localModel.Model);
        //}

        //[Obsolete]
        //public string Render(AbstractNode node)
        //{
        //    throw new InvalidOperationException();
        //}

        public ContentRenderer(IHost host, IRendererFactory rendererFactory) : base(host, rendererFactory) { }

        public override void Render(StringWriter writer, Statement statement, IDictionary<string, object> documentHost, object model)
        {
            var childrenQueue = documentHost.GetValueOrDefault("_LayoutChildren_") as Queue<StatementList>;
            if (childrenQueue == null)
            {
                //TODO: replace this with a real exception
                throw new Exception("Children elements empty");
            }

            var children = childrenQueue.Dequeue();

            RenderChildren(writer, children, documentHost, DefaultChildTag, model);
        }
    }
}