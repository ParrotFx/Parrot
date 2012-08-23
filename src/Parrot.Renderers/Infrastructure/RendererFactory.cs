using System;
using System.Collections.Generic;
using Parrot.Nodes;

namespace Parrot.Renderers.Infrastructure
{
    using Parrot.Infrastructure;

    public class RendererFactory : IRendererFactory
    {
        readonly Dictionary<string, IRenderer> _renderers;

        public RendererFactory(IHost host)
        {
            _renderers = new Dictionary<string, IRenderer>();

            //register default renderers
            RegisterFactory(new[] { "base", "basefont", "frame", "link", "meta", "area", "br", "col", "hr", "img", "param" }, new SelfClosingRenderer(host));
            RegisterFactory("doctype", new DocTypeRenderer());
            RegisterFactory("rawoutput", new RawOutputRenderer());
            RegisterFactory("output", new OutputRenderer());
            RegisterFactory("input", new InputRenderer());
            RegisterFactory("string", new StringLiteralRenderer());
            RegisterFactory("foreach", new ForeachRenderer(host));
            RegisterFactory("ul", new UlRenderer(host));

            //default renderer
            RegisterFactory("*", new HtmlRenderer(host));
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
            if (string.IsNullOrEmpty(blockName))
            {
                throw new ArgumentNullException("blockName");
            }

            if (renderer == null)
            {
                throw new ArgumentNullException("renderer");
            }

            if (!_renderers.ContainsKey(blockName))
            {
                _renderers.Add(blockName, renderer);
            }
        }

        public IRenderer GetRenderer(string blockName)
        {
            if (blockName != null)
            {
                if (_renderers.ContainsKey(blockName))
                {
                    return _renderers[blockName];
                }
            }

            return _renderers["*"];
        }
    }
}
