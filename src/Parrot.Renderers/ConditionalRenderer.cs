namespace Parrot.Renderers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Parrot.Infrastructure;
    using Parrot.Nodes;
    using Parrot.Renderers.Infrastructure;

    public class ConditionalRenderer : HtmlRenderer
    {
        public ConditionalRenderer(IHost host) : base(host)
        {
        }

        public override IEnumerable<string> Elements
        {
            get { yield return "conditional"; }
        }

        public override void Render(IParrotWriter writer, IRendererFactory rendererFactory, Statement statement, IDictionary<string, object> documentHost, object model)
        {
            var localModel = GetLocalModel(documentHost, statement, model);

            string statementToOutput = "default";

            if (localModel != null)
            {
                statementToOutput = localModel.ToString();
            }

            //check that there is a parameter

            foreach (var child in statement.Children)
            {
                string value;
                var valueWriter = Host.CreateWriter();

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