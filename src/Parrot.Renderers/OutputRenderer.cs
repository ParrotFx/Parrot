using System;
using Parrot.Renderers.Infrastructure;
using Parrot.Nodes;

namespace Parrot.Renderers
{
    using Parrot.Infrastructure;

    public class OutputRenderer : IRenderer
    {
        private readonly IHost _host;

        public OutputRenderer(IHost host)
        {
            _host = host;
        }

        public string Render(AbstractNode node, object model)
        {
            var modelValueProviderFactory = _host.DependencyResolver.Resolve<IModelValueProviderFactory>();

            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            var outputNode = node as EncodedOutput;
            if (outputNode == null)
            {
                throw new ArgumentNullException("node");
            }

            var value = modelValueProviderFactory.Get(model.GetType()).GetValue(model, Parrot.Infrastructure.ValueType.Property, outputNode.VariableName);

            //html encode this!
            return System.Net.WebUtility.HtmlEncode(value.ToString());
        }

        public string Render(AbstractNode node)
        {
            return Render(node, null);
        }
    }
}