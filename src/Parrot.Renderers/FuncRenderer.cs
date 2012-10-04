using System;
using System.Collections.Generic;
using System.IO;
using Parrot.Nodes;
using Parrot.Renderers.Infrastructure;

namespace Parrot.Renderers
{
    public class FuncRenderer : IRenderer
    {
        readonly Func<Statement, StringWriter, object, object, IRenderer> _renderer;

        public FuncRenderer(Func<Statement, StringWriter, object, object, IRenderer> renderer)
        {
            _renderer = renderer;
        }

        public void Render(StringWriter writer, Statement statement, IDictionary<string, object> documentHost, object model)
        {
            _renderer(statement, writer, documentHost, model);
        }
    }
}