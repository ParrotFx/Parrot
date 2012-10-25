using System.Collections.Generic;
using Parrot.Infrastructure;

namespace Parrot.Renderers.Infrastructure
{
    using System.IO;
    using Parrot.Nodes;

    public interface IRenderer
    {
        void Render(IParrotWriter writer, Statement statement, IDictionary<string, object> documentHost, object model);
    }
}