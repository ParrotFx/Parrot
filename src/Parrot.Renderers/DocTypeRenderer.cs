using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Parrot.Infrastructure;
using Parrot.Renderers.Infrastructure;
using Parrot.Nodes;

namespace Parrot.Renderers
{
    public class DocTypeRenderer : BaseRenderer, IRenderer
    {
        public DocTypeRenderer(IHost host) : base(host) { }

        public void Render(IParrotWriter writer, Statement statement, IDictionary<string, object> documentHost, object model)
        {
            var value = "html"; //default value of "html"

            var parameter = statement.Parameters.FirstOrDefault();
            if (parameter != null)
            {
                value = parameter.Value;
            }

            writer.Write(string.Format("<!DOCTYPE {0}>", value));
        }
    }
}