using System.Collections.Generic;

namespace Parrot.Renderers.Infrastructure
{
    using System.IO;
    using Parrot.Nodes;

    public interface IRenderer
    {
        void Render(StringWriter writer, Statement statement, IDictionary<string, object> documentHost, object model);
    }
}