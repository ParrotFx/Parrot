using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Parrot.Nodes;

namespace Parrot
{
    public interface IRendererFactory
    {
        void RegisterFactory(string blockName, Func<AbstractNode, object, string> renderer);
        void RegisterFactory(string[] blocks, Func<AbstractNode, object, string> renderer);
        void RegisterFactory(string blockName, IRenderer renderer);
        void RegisterFactory(string[] blocks, IRenderer renderer);
        IRenderer GetRenderer(string blockName);
    }

    public class RendererFactory : IRendererFactory
    {
        readonly Dictionary<string, IRenderer> _renderers;

        public RendererFactory()
        {
            _renderers = new Dictionary<string, IRenderer>();
        }

        public void RegisterFactory(string[] blocks, Func<AbstractNode, object, string> renderer)
        {
            foreach (var block in blocks)
            {
                RegisterFactory(block, renderer);
            }
        }

        public void RegisterFactory(string blockName, Func<AbstractNode, object, string> renderer)
        {
            _renderers.Add(blockName, new FuncRenderer(renderer));
        }

        public void RegisterFactory(string[] blocks, IRenderer renderer)
        {
            foreach (var block in blocks)
            {
                RegisterFactory(block, renderer);
            }
        }

        public void RegisterFactory(string blockName, IRenderer renderer)
        {
            if (!_renderers.ContainsKey(blockName))
            {
                _renderers.Add(blockName, renderer);
            }
        }

        public IRenderer GetRenderer(string blockName)
        {
            if (_renderers.ContainsKey(blockName))
            {
                return _renderers[blockName];
            }

            return _renderers["*"];
        }
    }


    public class FuncRenderer : IRenderer
    {
        readonly Func<AbstractNode, object, string> _renderer;

        public FuncRenderer(Func<AbstractNode, object, string> renderer)
        {
            _renderer = renderer;
        }

        public string Render(AbstractNode node, object model)
        {
            return _renderer(null, model);
        }

        public string Render(AbstractNode node)
        {
            return Render(node, null);
        }
    }

    public interface IRenderer
    {
        string Render(AbstractNode node, object model);
        string Render(AbstractNode node);
    }

}
