using System;
using System.IO;
using Parrot.Infrastructure;
using Parrot.Nodes;

namespace Parrot.Renderers.Infrastructure
{
    public interface IRendererFactory
    {
        void RegisterFactory(IRenderer renderer);
        IRenderer GetRenderer(string blockName);
    }
}