using System;
using System.IO;
using Parrot.Infrastructure;
using Parrot.Nodes;

namespace Parrot.Renderers.Infrastructure
{
    public interface IRendererFactory
    {
        void RegisterFactory(string blockName, Func<Statement, IParrotWriter, object, object, IRenderer> renderer);
        void RegisterFactory(string[] blocks, Func<Statement, IParrotWriter, object, object, IRenderer> renderer);
        void RegisterFactory(string blockName, IRenderer renderer);
        void RegisterFactory(string[] blocks, IRenderer renderer);
        IRenderer GetRenderer(string blockName);
    }
}