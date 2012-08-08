using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Parrot.Infrastructure;
using Parrot.Nodes;

namespace Parrot
{
    public interface IRendererFactory
    {
        void RegisterFactory(string blockName, Func<AbstractNode, object, LocalsStack, string> renderer);
        void RegisterFactory(string[] blocks, Func<AbstractNode, object, LocalsStack, string> renderer);
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

        public void RegisterFactory(string[] blocks, Func<AbstractNode, object, LocalsStack, string> renderer)
        {
            foreach (var block in blocks)
            {
                RegisterFactory(block, renderer);
            }
        }

        public void RegisterFactory(string blockName, Func<AbstractNode, object, LocalsStack, string> renderer)
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
        readonly Func<AbstractNode, object, LocalsStack, string> _renderer;

        public FuncRenderer(Func<AbstractNode, object, LocalsStack, string> renderer)
        {
            _renderer = renderer;
        }
        
        public string Render(AbstractNode node, object model, LocalsStack stack)
        {
            return _renderer(null, model, stack);
        }

        public string Render(AbstractNode node, LocalsStack stack)
        {
            return Render(node, null, stack);
        }
    }

    public interface IRenderer
    {
        string Render(AbstractNode node, LocalsStack stack);
        string Render(AbstractNode node, object model, LocalsStack stack);
    }

}
