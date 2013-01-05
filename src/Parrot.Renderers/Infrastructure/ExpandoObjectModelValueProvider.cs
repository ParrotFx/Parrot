namespace Parrot.Renderers.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using Parrot.Infrastructure;

    public class ExpandoObjectModelValueProvider : IModelValueProvider
    {
        private readonly IValueTypeProvider _valueTypeProvider;

        public ExpandoObjectModelValueProvider(IValueTypeProvider valueTypeProvider)
        {
            _valueTypeProvider = valueTypeProvider;
        }

        public bool GetValue(IDictionary<string, object> documentHost, object model, object property, out object value)
        {
            var valueType = GetValueType(ref property);

            return GetValue(documentHost, model, valueType, property, out value);
        }

        public bool GetValue(IDictionary<string, object> documentHost, object model, Parrot.Infrastructure.ValueType valueType, object property, out object value)
        {
            switch (valueType)
            {
                case Parrot.Infrastructure.ValueType.StringLiteral:
                case Parrot.Infrastructure.ValueType.Keyword:
                    value = property;
                    return true;
                case Parrot.Infrastructure.ValueType.Local:
                    value = model;
                    return true;
                case Parrot.Infrastructure.ValueType.Property:
                    if (model == null)
                    {
                        throw new NullReferenceException("model");
                    }

                    var propertyValues = (IDictionary<string, object>) model;
                    var result = model;

                    var properties = property.ToString().Split(".".ToCharArray());
                    for (int i = 0; i < properties.Length; i++)
                    {
                        var name = properties[i];
                        result = propertyValues[name];
                        propertyValues = result as IDictionary<string, object>;
                    }

                    value = result;
                    return true;
            }

            value = null;
            return false;
        }

        private Parrot.Infrastructure.ValueType GetValueType(ref object property)
        {
            var result = _valueTypeProvider.GetValue((string) property);

            property = result.Value.ToString();

            return result.Type;
        }
    }
}