namespace Parrot.Renderers.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Parrot.Infrastructure;

    public class ObjectModelValueProvider : IModelValueProvider
    {
        private readonly IValueTypeProvider _valueTypeProvider;

        public ObjectModelValueProvider(IValueTypeProvider valueTypeProvider)
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
                    if (model != null && GetModelProperty(model, property, out value))
                    {
                        return true;
                    }

                    if (documentHost != null && GetModelProperty(documentHost, property, out value))
                    {
                        return true;
                    }

                    value = null;
                    return false;
            }

            value = model;
            return false;
        }

        private Parrot.Infrastructure.ValueType GetValueType(ref object property)
        {
            if (property != null)
            {
                var result = _valueTypeProvider.GetValue((string) property);

                property = result.Value ?? null;

                return result.Type;
            }

            return Parrot.Infrastructure.ValueType.Keyword;
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

                if (model is IDictionary<string, object>)
                {
                    var host = (model as IDictionary<string, object>);
                    if (host.ContainsKey(parameters[0]))
                    {
                        var tempObject = host[parameters[0]];
                        return GetModelProperty(tempObject, string.Join(".", parameters.Skip(1)), out value);
                    }

                    //check the locals property
                    if (host.ContainsKey(Locals.LocalsKey))
                    {
                        List<object> locals = host[Locals.LocalsKey] as List<object>;
                        for (int i = locals.Count - 1; i >= 0; i--)
                        {
                            var local = locals[i];
                            if (GetModelProperty(local, property, out value))
                            {
                                return true;
                            }
                        }
                    }
                }
                else
                {
                    if (parameters[0].Length > 0)
                    {
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
                    else
                    {
                        value = model;
                        return true;
                    }
                }
            }
            value = null;
            return false;
        }
    }
}