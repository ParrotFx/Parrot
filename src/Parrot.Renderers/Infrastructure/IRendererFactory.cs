using System;
using Parrot.Nodes;

namespace Parrot.Renderers.Infrastructure
{
    public interface IRendererFactory
    {
        void RegisterFactory(string blockName, Func<AbstractNode, object, string> renderer);
        void RegisterFactory(string[] blocks, Func<AbstractNode, object, string> renderer);
        void RegisterFactory(string blockName, IRenderer renderer);
        void RegisterFactory(string[] blocks, IRenderer renderer);
        IRenderer GetRenderer(string blockName);
    }
}