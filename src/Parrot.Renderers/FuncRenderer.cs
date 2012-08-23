using System;
using Parrot.Nodes;

namespace Parrot.Renderers.Infrastructure
{
    public class FuncRenderer : IRenderer
    {
        readonly Func<AbstractNode, object, string> _renderer;

        public FuncRenderer(Func<AbstractNode, object, string> renderer)
        {
            _renderer = renderer;
        }

        public string Render(AbstractNode node, object model)
        {
            return _renderer(null, model);
        }

        public string Render(AbstractNode node)
        {
            return Render(node, null);
        }
    }
}