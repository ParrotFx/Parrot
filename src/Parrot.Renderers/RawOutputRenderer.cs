using System;
using System.Collections.Generic;
using System.IO;
using Parrot.Renderers.Infrastructure;
using Parrot.Nodes;

namespace Parrot.Renderers
{
    using Parrot.Infrastructure;

    public class RawOutputRenderer : IRenderer
    {
        private IHost _host;

        public RawOutputRenderer(IHost host)
        {
            _host = host;
        }

        private string GetModelValue(IModelValueProviderFactory factory, object model, StringLiteralPartType type, string data)
        {
            IValueTypeProvider valueTypeProvider = _host.DependencyResolver.Resolve<IValueTypeProvider>();
            var valueType = valueTypeProvider.GetValue(data);

            object value = null;
            switch (type)
            {
                case StringLiteralPartType.Encoded:
                    //get the valuetype
                    if (factory.Get(model.GetType()).GetValue(model, valueType.Type, data, out value))
                    {
                        return System.Net.WebUtility.HtmlEncode(value.ToString());
                    }
                    break;
                case StringLiteralPartType.Raw:
                    if (factory.Get(model.GetType()).GetValue(model, valueType.Type, data, out value))
                    {
                        return value.ToString();
                    }
                    break;
            }

            //default type is string literal
            return data;
        }
        
        public void Render(StringWriter writer, Statement statement, IDictionary<string, object> documentHost, object model)
        {
            var modelValueProviderFactory = _host.DependencyResolver.Resolve<IModelValueProviderFactory>();

            var value = GetModelValue(modelValueProviderFactory, model, StringLiteralPartType.Raw, statement.Name);

            writer.Write(value);
        }
    }
}