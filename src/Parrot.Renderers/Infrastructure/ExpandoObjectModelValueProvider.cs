namespace Parrot.Renderers.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using ValueType = Parrot.Infrastructure.ValueType;

    public class ExpandoObjectModelValueProvider : IModelValueProvider
    {
        public object GetValue(object model, ValueType valueType, object property)
        {
            switch (valueType)
            {
                case Parrot.Infrastructure.ValueType.StringLiteral:
                case Parrot.Infrastructure.ValueType.Keyword:
                    return property;
                case Parrot.Infrastructure.ValueType.Local:
                    return model;
                case Parrot.Infrastructure.ValueType.Property:
                    if (model == null)
                    {
                        throw new NullReferenceException("model");
                    }

                    var propertyValues = (IDictionary<string, object>)model;
                    var result = model;

                    var properties = property.ToString().Split(".".ToCharArray());
                    for (int i = 0; i < properties.Length; i++)
                    {
                        var name = properties[i];
                        result = propertyValues[name];
                        propertyValues = result as IDictionary<string, object>;
                    }

                    return result;
            }

            throw new InvalidOperationException("ValueType");
        }

    }
}