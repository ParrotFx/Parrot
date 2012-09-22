namespace Parrot.Mvc.Renderers
{
    using Parrot.Infrastructure;
    using System;
    using Parrot.Nodes;
    using Parrot.Renderers;
    using Parrot.Renderers.Infrastructure;

    public class ContentRenderer : IRenderer
    {
        private readonly IHost _host;

        public ContentRenderer(IHost host)
        {
            _host = host;
        }

        public string Render(AbstractNode node, object model)
        {
            dynamic localModel = model;

            Document document = new Document(_host)
            {
                Children = localModel.Children
            };

            return _host.DependencyResolver.Resolve<DocumentRenderer>().Render(document, localModel.Model);
        }

        [Obsolete]
        public string Render(AbstractNode node)
        {
            throw new InvalidOperationException();
        }
    }
}