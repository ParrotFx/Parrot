using System;
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

        public string Render(AbstractNode node, object model)
        {
            var modelValueProviderFactory = _host.DependencyResolver.Resolve<IModelValueProviderFactory>();

            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            var stringNode = node as StringLiteral;
            if (stringNode == null)
            {
                throw new InvalidCastException("node");
            }

            string result = "";
            foreach (var value in stringNode.Values)
            {
                result += GetModelValue(modelValueProviderFactory, model, value.Type, value.Data);
            }

            return result;
            //return stringNode.GetValue() as string;
        }

        public string Render(AbstractNode node)
        {
            return Render(node, null);
        }

        private string GetModelValue(IModelValueProviderFactory factory, object model, StringLiteralPartType type, string data)
        {
            IValueTypeProvider valueTypeProvider = _host.DependencyResolver.Resolve<IValueTypeProvider>();
            var valueType = valueTypeProvider.GetValue(data);
            
            switch (type)
            {
                case StringLiteralPartType.Encoded:
                    //get the valuetype

                    return System.Net.WebUtility.HtmlEncode(factory.Get(model.GetType()).GetValue(model, valueType.Type, data).ToString());
                case StringLiteralPartType.Raw:
                    return factory.Get(model.GetType()).GetValue(model, valueType.Type, data).ToString();
            }

            //default type is string literal
            return data;
        }
    }
}