using System;
using Parrot.Renderers.Infrastructure;
using Parrot.Nodes;

namespace Parrot.Renderers
{
    using System.Web;
    using Parrot.Infrastructure;

    public class StringLiteralRenderer : IRenderer
    {
        private IHost _host;

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

            //return RendererHelpers.GetModelValue(model, stringNode.Parrot.Infrastructure.ValueType, stringNode)

            return result;
            //return stringNode.GetValue() as string;
        }

        public string Render(AbstractNode node)
        {
            return Render(node, null);
        }

        private string GetModelValue(IModelValueProviderFactory factory, object model, StringLiteralPartType type, string data)
        {
            switch (type)
            {
                case StringLiteralPartType.Encoded:
                    return System.Net.WebUtility.HtmlEncode((string)factory.Get(model.GetType()).GetValue(model, Parrot.Infrastructure.ValueType.Property, data));
                case StringLiteralPartType.Raw:
                    return (string)factory.Get(model.GetType()).GetValue(model, Parrot.Infrastructure.ValueType.Property, data);
            }

            //default type is string literal
            return data;
        }
    }
}