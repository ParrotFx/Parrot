// -----------------------------------------------------------------------
// <copyright file="ForeachRenderer.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using Parrot.Renderers.Infrastructure;

namespace Parrot.Renderers
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Text;
    using Nodes;
    using Parrot.Infrastructure;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ForeachRenderer : HtmlRenderer
    {
        public ForeachRenderer(IHost host, IRendererFactory rendererFactory) : base(host, rendererFactory) { }

        public override void Render(IParrotWriter writer, Statement statement, IDictionary<string, object> documentHost, object model)
        {
            Type modelType = model != null ? model.GetType() : null;
            var modelValueProvider = ModelValueProviderFactory.Get(modelType);

            var localModel = GetLocalModelValue(documentHost, statement, modelValueProvider, model);

            //Assert that we're looping over something
            IEnumerable loop = localModel as IEnumerable;
            if (loop == null)
            {
                throw new InvalidCastException("model is not IEnumerable");
            }

            foreach (var item in loop)
            {

                foreach (var child in statement.Children)
                {
                    var renderer = RendererFactory.GetRenderer(child.Name);
                    renderer.Render(writer, child, documentHost, item);
                }
            }
        }
    }
}
