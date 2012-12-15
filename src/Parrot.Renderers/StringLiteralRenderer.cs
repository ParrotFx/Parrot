using System;
using System.Collections.Generic;
using System.IO;
using Parrot.Renderers.Infrastructure;
using Parrot.Nodes;

namespace Parrot.Renderers
{
    using System.Web;
    using Parrot.Infrastructure;

    public class StringLiteralRenderer : IRenderer
    {
        private readonly IHost _host;

        public StringLiteralRenderer(IHost host)
        {
            _host = host;
        }

        private string GetModelValue(IModelValueProviderFactory factory, IDictionary<string, object> documentHost, object model, StringLiteralPartType type, string data)
        {
            IValueTypeProvider valueTypeProvider = _host.DependencyResolver.Resolve<IValueTypeProvider>();
            var valueType = valueTypeProvider.GetValue(data);

            object value = null;
            switch (type)
            {
                case StringLiteralPartType.Encoded:
                    //get the valuetype
                    if (factory.Get(model.GetType()).GetValue(documentHost, model, valueType.Type, data, out value))
                    {
                        return System.Net.WebUtility.HtmlEncode(value.ToString());
                    }
                    break;
                case StringLiteralPartType.Raw:
                    if (factory.Get(model.GetType()).GetValue(documentHost, model, valueType.Type, data, out value))
                    {
                        return value.ToString();
                    }
                    break;
            }

            //default type is string literal
            return data;
        }

        public void Render(IParrotWriter writer, IRendererFactory rendererFactory, Statement statement, IDictionary<string, object> documentHost, object model)
        {
            var modelValueProviderFactory = _host.DependencyResolver.Resolve<IModelValueProviderFactory>();

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
    }
}