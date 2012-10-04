using System;
using System.IO;
using Parrot.Nodes;

namespace Parrot.Renderers.Infrastructure
{
    public interface IRendererFactory
    {
        void RegisterFactory(string blockName, Func<Statement, StringWriter, object, object, IRenderer> renderer);
        void RegisterFactory(string[] blocks, Func<Statement, StringWriter, object, object, IRenderer> renderer);
        void RegisterFactory(string blockName, IRenderer renderer);
        void RegisterFactory(string[] blocks, IRenderer renderer);
        IRenderer GetRenderer(string blockName);
    }
}