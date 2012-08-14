using Parrot.Infrastructure;

namespace Parrot.Mvc.Renderers
{
    using System;
    using Nodes;

    public class ContentRenderer : IRenderer
    {
        public string Render(AbstractNode node, object model)
        {
            dynamic localModel = model;

            Document document = new Document
            {
                Children = localModel.Children
            };

            return document.Render(localModel.Model);
        }

        [Obsolete]
        public string Render(AbstractNode node)
        {
            throw new InvalidOperationException();
        }
    }
}