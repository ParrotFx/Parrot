namespace Parrot.Renderers
{
    using System;
    using System.Collections.Generic;
    using Parrot.Infrastructure;
    using Parrot.Nodes;
    using Parrot.Renderers.Infrastructure;

    public class ParameterRenderer : IRenderer
    {
        private readonly IHost _host;

        public ParameterRenderer(IHost host)
        {
            _host = host;
        }

        public IEnumerable<string> Elements { get; private set; }

        public void Render(IParrotWriter writer, IRendererFactory rendererFactory, Statement statement, IDictionary<string, object> documentHost, object model)
        {
            throw new NotImplementedException();
        }
    }
}