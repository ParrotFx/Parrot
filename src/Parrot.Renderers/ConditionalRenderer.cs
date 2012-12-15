
namespace Parrot.Renderers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Parrot.Infrastructure;
    using Nodes;
    using Infrastructure;

    public class ConditionalRenderer : HtmlRenderer
    {
        public ConditionalRenderer(IHost host) : base(host) { }

        public override void Render(IParrotWriter writer, IRendererFactory rendererFactory, Statement statement, IDictionary<string, object> documentHost, object model)
        {
            Type modelType = model != null ? model.GetType() : null;
            var modelValueProvider = ModelValueProviderFactory.Get(modelType);

            var localModel = GetLocalModelValue(documentHost, statement, modelValueProvider, model);

            string statementToOutput = "default";

            if (localModel != null)
            {
                statementToOutput = localModel.ToString();
            }

            //check that there is a parameter

            foreach (var child in statement.Children)
            {
                string value = null;
                var valueWriter = Host.DependencyResolver.Resolve<IParrotWriter>();

                //get string value
                var renderer = rendererFactory.GetRenderer(child.Name);
                if (renderer is StringLiteralRenderer)
                {
                    renderer.Render(valueWriter, rendererFactory, child, documentHost, model);
                    value = valueWriter.Result();
                }
                else
                {
                    value = child.Name;
                }

                if (value.Equals(statementToOutput, StringComparison.OrdinalIgnoreCase))
                {
                    //render only the child
                    RenderChildren(writer, child, rendererFactory, documentHost, model);
                    return;
                }
            }

            var defaultChild = statement.Children.SingleOrDefault(s => s.Name.Equals("default", StringComparison.OrdinalIgnoreCase));
            if (defaultChild != null)
            {
                RenderChildren(writer, defaultChild, rendererFactory, documentHost, model);
            }
        }
    }
}