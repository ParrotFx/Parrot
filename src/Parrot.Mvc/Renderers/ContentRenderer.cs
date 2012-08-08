using Parrot.Infrastructure;

namespace Parrot.Mvc.Renderers
{
    using System;
    using Nodes;

    public class ContentRenderer : IRenderer
    {
        public string Render(AbstractNode node, LocalsStack stack)
        {
            throw new NotImplementedException();
        }

        public string Render(AbstractNode node, object model, LocalsStack stack)
        {
            dynamic localModel = model;

            Document document = new Document
            {
                Children = localModel.Children
            };

            return document.Render(localModel.Model, stack);
        }
    }
}