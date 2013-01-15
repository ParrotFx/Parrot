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
        private static Dictionary<string, Func<string>> _docTypes;

        static DocTypeRenderer()
        {
            _docTypes = new Dictionary<string, Func<string>>()
                {
                    {"html", () => "html"},
                    {"html 4.01 strict", () => "HTML PUBLIC \"-//W3C//DTD HTML 4.01//EN\" \"http://www.w3.org/TR/html4/strict.dtd\""},
                    {"html 4.01 transitional", () => "HTML PUBLIC \"-//W3C//DTD HTML 4.01 Transitional//EN\" \"http://www.w3.org/TR/html4/loose.dtd\""},
                    {"html 4.01 frameset", () => "HTML PUBLIC \"-//W3C//DTD HTML 4.01 Frameset//EN\" \"http://www.w3.org/TR/html4/frameset.dtd\""},
                    {"xhtml 1.0 strict", () => "html PUBLIC \"-//W3C//DTD XHTML 1.0 Strict//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd\"" },
                    {"xhtml 1.0 transitional", () => "html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\"" },
                    {"xhtml 1.0 frameset", () => "html PUBLIC \"-//W3C//DTD XHTML 1.0 Frameset//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-frameset.dtd\"" },
                    {"xhtml 1.1", () => "html PUBLIC \"-//W3C//DTD XHTML 1.1//EN\" \"http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd\"" },
                    {"xhtml 1.1 basic", () => "html PUBLIC \"-//W3C//DTD XHTML Basic 1.1//EN\" \"http://www.w3.org/TR/xhtml-basic/xhtml-basic11.dtd\"" },
                };
        }

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

            if (_docTypes.ContainsKey(value))
            {
                value = _docTypes[value]();
            }

            documentHost.Add("doctype", value);

            writer.Write(string.Format("<!DOCTYPE {0}>", value));
        }

        protected override IHost Host { get; set; }
    }
}