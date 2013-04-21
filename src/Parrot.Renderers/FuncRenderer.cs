namespace Parrot.Renderers
{
    using System;
    using System.Collections.Generic;
    using Parrot.Infrastructure;
    using Parrot.Nodes;
    using Parrot.Renderers.Infrastructure;

    public class FuncRenderer : IRenderer
    {
        private readonly Func<Statement, IParrotWriter, object, object, IRenderer> _renderer;

        public FuncRenderer(Func<Statement, IParrotWriter, object, object, IRenderer> renderer)
        {
            _renderer = renderer;
        }

        public IEnumerable<string> Elements
        {
            get { yield break; }
        }

        public void BeforeRender(IParrotWriter writer, IRendererFactory rendererFactory, Statement statement, IDictionary<string, object> documentHost, object model) { }

        public void Render(IParrotWriter writer, IRendererFactory rendererFactory, Statement statement, IDictionary<string, object> documentHost, object model)
        {
            _renderer(statement, writer, documentHost, model);
        }

        public void AfterRender(IParrotWriter writer, IRendererFactory rendererFactory, Statement statement, IDictionary<string, object> documentHost, object model) { }
    }
}