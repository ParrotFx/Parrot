namespace Parrot.Renderers.Infrastructure
{
    using System;
    using System.Linq;

    public class ObjectModelValueProvider : IModelValueProvider
    {
        public bool GetValue(object model, Parrot.Infrastructure.ValueType valueType, object property, out object value)
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
                        value = null;
                        return false;
                    }

                    return GetModelProperty(model, property, out value);
            }

            value = model;
            return false;
        }

        private bool GetModelProperty(object model, object property, out object value)
        {
            if (property != null)
            {
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
                        value = tempObject;
                        return true;
                    }

                    return GetModelProperty(tempObject, string.Join(".", parameters.Skip(1)), out value);
                }
            }
            value = null;
            return false;
        }
    }
}