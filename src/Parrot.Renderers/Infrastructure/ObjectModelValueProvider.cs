namespace Parrot.Renderers.Infrastructure
{
    using System;
    using System.Linq;

    public class ObjectModelValueProvider : IModelValueProvider
    {
        public object GetValue(object model, Parrot.Infrastructure.ValueType valueType, object property)
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

                    return GetModelProperty(model, property);
            }

            throw new InvalidOperationException("ValueType");
        }

        private object GetModelProperty(object model, object property)
        {
            if (property == null)
            {
                throw new NullReferenceException("property");
            }

            var stringProperty = property.ToString();
            string[] parameters = stringProperty.Split(".".ToCharArray());

            if (model == null && parameters.Length != 1)
            {
                throw new NullReferenceException(parameters[0]);
            }

            var pi = model.GetType().GetProperty(parameters[0]);
            if (pi != null)
            {
                var tempObject = pi.GetValue(model, null);
                if (parameters.Length == 1)
                {
                    return tempObject;
                }

                return GetModelProperty(tempObject, string.Join(".", parameters.Skip(1)));
            }

            return null;
        }
    }
}