// -----------------------------------------------------------------------
// <copyright file="BaseRenderer.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Parrot.Renderers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Parrot.Infrastructure;
    using Parrot.Nodes;
    using Parrot.Renderers.Infrastructure;

    public abstract class BaseRenderer
    {
        protected abstract IHost Host { get; set; }

        protected object GetLocalModelValue(IDictionary<string, object> documentHost, Statement statement, IModelValueProvider modelValueProvider, object model)
        {
            object value;
            if (statement.Parameters.Count > 0)
            {
                //check here first
                if (modelValueProvider.GetValue(documentHost, model, statement.Parameters[0].Value, out value))
                {
                    return value;
                }
            }

            if (model != null)
            {
                //check DocumentHost next
                //if (modelValueProvider.GetValue(documentHost, model, null, out value))
                if (modelValueProvider.GetValue(documentHost, model, Parrot.Infrastructure.ValueType.Property, null, out value))
                {
                    return value;
                }
            }
            return model;
        }

        public void BeforeRender(IParrotWriter writer, IRendererFactory rendererFactory, Statement statement, IDictionary<string, object> documentHost, object model)
        {
            //process parameters
            if (statement.Parameters != null && statement.Parameters.Any())
            {
                foreach (var parameter in statement.Parameters)
                {
                    if (parameter.Value != null && ((parameter.Value.StartsWith("\"") && parameter.Value.EndsWith("\"")) || (parameter.Value.StartsWith("'") && parameter.Value.EndsWith("'"))))
                    {
                        var stringLiteral = new StringLiteral(parameter.Value);
                        var renderer = rendererFactory.GetRenderer("string");
                        var w = new StandardWriter();
                        {
                            renderer.Render(w, rendererFactory, stringLiteral, documentHost, model);
                        }

                        parameter.CalculatedValue = w.Result();
                    }
                }
            }
        }

        public void AfterRender(IParrotWriter writer, IRendererFactory rendererFactory, Statement statement, IDictionary<string, object> documentHost, object model) { }
    }

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public abstract class BaseRenderer<T>
    {
        protected T RenderingHost;

        //needs a way to override property resolution
    }

    public class ModelRenderingHost: IRenderingHost
    {
        public object Model { get; set; }
    }

    public interface IRenderingHost { }
}