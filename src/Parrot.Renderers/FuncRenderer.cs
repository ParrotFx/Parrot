using System;
using System.Collections.Generic;
using System.IO;
using Parrot.Infrastructure;
using Parrot.Nodes;
using Parrot.Renderers.Infrastructure;

namespace Parrot.Renderers
{
    public class FuncRenderer : IRenderer
    {
        readonly Func<Statement, IParrotWriter, object, object, IRenderer> _renderer;

        public FuncRenderer(Func<Statement, IParrotWriter, object, object, IRenderer> renderer)
        {
            _renderer = renderer;
        }

        public IEnumerable<string> Elements
        {
            get
            {
                yield break;
            }
        }

        public void Render(IParrotWriter writer, IRendererFactory rendererFactory, Statement statement, IDictionary<string, object> documentHost, object model)
        {
            _renderer(statement, writer, documentHost, model);
        }
    }
}