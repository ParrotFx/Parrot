namespace Parrot.Renderers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Parrot.Infrastructure;
    using Parrot.Nodes;
    using Parrot.Renderers.Infrastructure;

    public class DocTypeRenderer : BaseRenderer, IRenderer
    {
        public DocTypeRenderer(IHost host)
        {
            Host = host;
        }

        public IEnumerable<string> Elements
        {
            get { yield return "doctype"; }
        }

        public void Render(IParrotWriter writer, IRendererFactory rendererFactory, Statement statement, IDictionary<string, object> documentHost, object model)
        {
            var value = "html"; //default value of "html"

            var parameter = statement.Parameters.FirstOrDefault();
            if (parameter != null)
            {
                Type modelType = model != null ? model.GetType() : null;
                var modelValueProvider = Host.ModelValueProviderFactory.Get(modelType);

                object result;
                if (modelValueProvider.GetValue(documentHost, model, statement.Parameters[0].Value, out result))
                {
                    value = result.ToString();
                }
            }

            writer.Write(string.Format("<!DOCTYPE {0}>", value));
        }

        protected override IHost Host { get; set; }
    }
}