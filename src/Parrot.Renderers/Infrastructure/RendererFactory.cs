namespace Parrot.Renderers.Infrastructure
{
    using System;
    using System.Collections.Generic;

    public class RendererFactory : IRendererFactory
    {
        private readonly Dictionary<string, IRenderer> _renderers;

        public RendererFactory(IEnumerable<IRenderer> renderers)
        {
            _renderers = new Dictionary<string, IRenderer>();
            foreach (var renderer in renderers)
            {
                RegisterFactory(renderer);
            }
        }

        public void RegisterFactory(IRenderer renderer)
        {
            foreach (var block in renderer.Elements)
            {
                RegisterFactory(block, renderer);
            }
        }

        private void RegisterFactory(string blockName, IRenderer renderer)
        {
            if (string.IsNullOrEmpty(blockName))
            {
                throw new ArgumentNullException("blockName");
            }

            if (renderer == null)
            {
                throw new ArgumentNullException("renderer");
            }

            if (_renderers.ContainsKey(blockName))
            {
                _renderers.Remove(blockName);
            }

            _renderers.Add(blockName, renderer);
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