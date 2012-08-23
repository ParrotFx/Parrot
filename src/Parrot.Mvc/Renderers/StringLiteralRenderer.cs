using System;
using Parrot.Infrastructure;
using Parrot.Nodes;
using ValueType = Parrot.Infrastructure.ValueType;

namespace Parrot.Mvc.Renderers
{
    using System.Web;
    using ValueType = ValueType;

    public class StringLiteralRenderer : IRenderer
    {
        public string Render(AbstractNode node, object model)
        {
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
                result += GetModelValue(model, value.Type, value.Data);
            }

            //return RendererHelpers.GetModelValue(model, stringNode.ValueType, stringNode)

            return result;
            //return stringNode.GetValue() as string;
        }

        public string Render(AbstractNode node)
        {
            return Render(node, null);
        }

        private string GetModelValue(object model, StringLiteralPartType type, string data)
        {
            switch (type)
            {
                case StringLiteralPartType.Encoded:
                    return System.Net.WebUtility.HtmlEncode((string)RendererHelpers.GetModelValue(model, ValueType.Property, data));
                case StringLiteralPartType.Raw:
                    return (string)RendererHelpers.GetModelValue(model, ValueType.Property, data);
            }

            //default type is string literal
            return data;
        }
    }
}