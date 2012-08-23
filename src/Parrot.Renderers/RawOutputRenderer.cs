using System;
using Parrot.Renderers.Infrastructure;
using Parrot.Nodes;

namespace Parrot.Renderers
{
    using Parrot.Infrastructure;

    public class RawOutputRenderer : IRenderer
    {
        private IHost _host;

        public RawOutputRenderer(IHost host)
        {
            _host = host;
        }

        public string Render(AbstractNode node, object model)
        {
            var modelValueProviderFactory = _host.DependencyResolver.Get<IModelValueProviderFactory>();

            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            var outputNode = node as RawOutput;
            if (outputNode == null)
            {
                throw new ArgumentNullException("node");
            }

            var value = modelValueProviderFactory.Get(model.GetType()).GetValue(model, Parrot.Infrastructure.ValueType.Property, outputNode.VariableName);

            return value.ToString();
        }

        public string Render(AbstractNode node)
        {
            return Render(node, null);
        }
    }
}