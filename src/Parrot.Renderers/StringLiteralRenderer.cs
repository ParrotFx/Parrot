namespace Parrot.Renderers
{
    using System;
    using System.Collections.Generic;
    using Parrot.Infrastructure;
    using Parrot.Nodes;
    using Parrot.Renderers.Infrastructure;

    public class StringLiteralRenderer : IRenderer
    {
        private readonly IHost _host;

        public StringLiteralRenderer(IHost host)
        {
            _host = host;
        }

        private string GetModelValue(IModelValueProviderFactory factory, IDictionary<string, object> documentHost, object model, StringLiteralPartType type, string data)
        {
            Type modelType = model != null ? model.GetType() : null;

            object value;
            switch (type)
            {
                case StringLiteralPartType.Encoded:
                    //get the valuetype
                    if (factory.Get(modelType).GetValue(documentHost, model, data, out value))
                    {
                        return System.Net.WebUtility.HtmlEncode(value.ToString());
                    }
                    break;
                case StringLiteralPartType.Raw:
                    if (factory.Get(modelType).GetValue(documentHost, model, data, out value))
                    {
                        return value.ToString();
                    }
                    break;
            }

            //default type is string literal
            return data;
        }

        public IEnumerable<string> Elements
        {
            get { yield return "string"; }
        }

        public void BeforeRender(IParrotWriter writer, IRendererFactory rendererFactory, Statement statement, IDictionary<string, object> documentHost, object model) { }

        public void Render(IParrotWriter writer, IRendererFactory rendererFactory, Statement statement, IDictionary<string, object> documentHost, object model)
        {
            var modelValueProviderFactory = _host.ModelValueProviderFactory;

            string result = "";
            if (statement is StringLiteral)
            {
                foreach (var value in (statement as StringLiteral).Values)
                {
                    result += GetModelValue(
                        modelValueProviderFactory,
                        documentHost,
                        model,
                        value.Type,
                        value.Data
                        );
                }
            }
            else
            {
                result = GetModelValue(modelValueProviderFactory, documentHost, model, StringLiteralPartType.Encoded, statement.Name);
            }

            writer.Write(result);
        }

        public void AfterRender(IParrotWriter writer, IRendererFactory rendererFactory, Statement statement, IDictionary<string, object> documentHost, object model) { }
    }
}