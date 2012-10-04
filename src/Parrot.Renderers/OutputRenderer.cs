using System;
using System.IO;
using Parrot.Renderers.Infrastructure;
using Parrot.Nodes;

namespace Parrot.Renderers
{
    using Parrot.Infrastructure;

    public class OutputRenderer : BaseRenderer, IRenderer
    {
        private readonly IHost _host;
        private readonly IRendererFactory _rendererFactory;

        public OutputRenderer(IHost host, IRendererFactory rendererFactory)
            : base(host)
        {
            _host = host;
            _rendererFactory = rendererFactory;
        }

        public string Render(AbstractNode node, object documentHost)
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

            var value = modelValueProviderFactory.Get(documentHost.GetType()).GetValue(documentHost, Parrot.Infrastructure.ValueType.Property, outputNode.VariableName);

            //html encode this!
            return System.Net.WebUtility.HtmlEncode(value.ToString());
        }

        public void Render(StringWriter writer, Statement statement, object documentHost)
        {
            var outputNode = statement as EncodedOutput;
            if (outputNode == null)
            {
                throw new ArgumentNullException("statement");

            }

            Type documentHostType = documentHost != null ? documentHost.GetType() : null;
            var modelValueProvider = ModelValueProviderFactory.Get(documentHostType);

            var value = modelValueProvider.GetValue(documentHost, ValueType.Property, statement);

            //html encode this!
            writer.Write(System.Net.WebUtility.HtmlEncode(value.ToString()));
        }
    }
}