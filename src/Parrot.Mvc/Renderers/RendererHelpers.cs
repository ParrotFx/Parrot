// -----------------------------------------------------------------------
// <copyright file="RendererHelpers.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using Parrot.Infrastructure;

namespace Parrot.Mvc.Renderers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Nodes;
    using ValueType = ValueType;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class RendererHelpers
    {
        internal static object GetModelValue(object model, ValueType valueType, object property)
        {
            switch (valueType)
            {
                case ValueType.Property:
                    //check to see if the property is any one of several keywords

                    if (model == null)
                    {
                        throw new NullReferenceException("model");
                    }

                    var stringProperty = property.ToString();

                    string[] parameters = stringProperty.Split(".".ToCharArray());

                    object modelToCheck = model;

                    if (stringProperty == "this")
                    {
                        return model;
                    }

                    if (model != null)
                    {
                        var pi = model.GetType().GetProperty(parameters[0]);
                        if (pi != null)
                        {
                            var tempObject = pi.GetValue(model, null);

                            if (parameters.Length == 1)
                            {
                                return tempObject;
                            }

                            return GetModelValue(tempObject, ValueType.Property, string.Join(".", parameters.Skip(1)));
                        }
                    }

                    break;
                case ValueType.StringLiteral:
                    return property;

                case ValueType.Keyword:
                    return property;

                case ValueType.Local:
                    return model;
            }

            return null;
        }

    }
}
