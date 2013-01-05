namespace Parrot.Renderers.Infrastructure
{
    using System.Collections.Generic;
    using Parrot.Infrastructure;
    using Parrot.Nodes;

    public interface IRenderer
    {
        IEnumerable<string> Elements { get; }
        void Render(IParrotWriter writer, IRendererFactory rendererFactory, Statement statement, IDictionary<string, object> documentHost, object model);
    }
}