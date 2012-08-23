// -----------------------------------------------------------------------
// <copyright file="RendererHelpers.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using Parrot.Renderers.Infrastructure;

namespace Parrot.Renderers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Nodes;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class RendererHelpers
    {
        //public static string Render(this Document document, object model)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    foreach (var element in document.Children)
        //    {
        //        var factory = Infrastructure.Host.DependencyResolver.Get<IRendererFactory>();

        //        var renderer = factory.GetRenderer(element.Name);
        //        sb.AppendLine(renderer.Render(element, model));
        //    }

        //    return sb.ToString();
        //}

        //public static string Render(this IList<Statement> nodes, object model)
        //{
        //    var factory = Infrastructure.Host.DependencyResolver.Get<IRendererFactory>();
        //    StringBuilder sb = new StringBuilder();
        //    foreach (var child in nodes)
        //    {
        //        sb.Append(factory.GetRenderer(child.Name).Render(child, model));
        //    }

        //    return sb.ToString();
        //}

        public static object GetModelValue(object model, Parrot.Infrastructure.ValueType valueType, object property)
        {
            switch (valueType)
            {
                case Parrot.Infrastructure.ValueType.Property:
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

                            return GetModelValue(tempObject, Parrot.Infrastructure.ValueType.Property, string.Join(".", parameters.Skip(1)));
                        }
                    }


                    break;
                case Parrot.Infrastructure.ValueType.StringLiteral:
                    return property;

                case Parrot.Infrastructure.ValueType.Keyword:
                    return property;

                case Parrot.Infrastructure.ValueType.Local:
                    return model;
            }

            return null;
        }

    }
}
