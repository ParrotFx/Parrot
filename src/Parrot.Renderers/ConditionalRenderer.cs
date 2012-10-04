using System;
using System.Collections.Generic;
using System.Linq;
using Parrot.Infrastructure;
using Parrot.Nodes;
using Parrot.Renderers.Infrastructure;

namespace Parrot.Renderers
{
    using System.IO;
    using System.Text;

    public class ConditionalRenderer : HtmlRenderer
    {
        public ConditionalRenderer(IHost host, IRendererFactory rendererFactory) : base(host, rendererFactory) { }

        public override void Render(System.IO.StringWriter writer, Statement statement, IDictionary<string, object> documentHost, object model)
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
                StringBuilder sb = new StringBuilder();
                StringWriter valueWriter = new StringWriter(sb);

                //get string value
                var renderer = RendererFactory.GetRenderer(child.Name);
                if (renderer is StringLiteralRenderer)
                {
                    renderer.Render(valueWriter, child, documentHost, localModel);
                    value = sb.ToString();
                }
                else
                {
                    value = child.Name;
                }

                if (value.Equals(statementToOutput, StringComparison.OrdinalIgnoreCase))
                {
                    var rendererFactory = Host.DependencyResolver.Resolve<IRendererFactory>();
                    //render only the child
                    RenderChildren(writer, child, documentHost, localModel);
                    return;
                }
            }

            var defaultChild = statement.Children.SingleOrDefault(s => s.Name.Equals("default", StringComparison.OrdinalIgnoreCase));
            if (defaultChild != null)
            {
                RenderChildren(writer, defaultChild, documentHost, localModel);
            }
        }
    }
}