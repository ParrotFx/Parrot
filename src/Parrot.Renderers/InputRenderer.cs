using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Parrot.Renderers.Infrastructure;
using Parrot.Nodes;

namespace Parrot.Renderers
{
    using Parrot.Infrastructure;
    using Parrot.Renderers;

    public class InputRenderer : SelfClosingRenderer, IRenderer
    {
        public InputRenderer(IHost host, IRendererFactory rendererFactory) : base(host, rendererFactory) { }

        public new void Render(StringWriter writer, Statement statement, IDictionary<string, object> documentHost, object model)
        {
            //get the input type
            string type = "checkbox";
            switch(type)
            {
                case "checkbox":
                    //special handling for "checked"
                    //is there a checked attribute
                    for (int i = 0; i < statement.Attributes.Count; i++)
                    {
                        if (statement.Attributes[i].Key == "checked")
                        {

                            if (statement.Attributes[i] != null)
                            {
                                var renderer = RendererFactory.GetRenderer(statement.Attributes[i].Value.Name);

                                if (renderer is HtmlRenderer)
                                {
                                    renderer = RendererFactory.GetRenderer("string");
                                }

                                //render attribute
                                var attributeRenderer = Host.DependencyResolver.Resolve<IAttributeRenderer>();
                                StringBuilder sb = new StringBuilder();
                                var tempWriter = new StringWriter(sb);
                                renderer.Render(tempWriter, statement.Attributes[i].Value, documentHost, model);

                                var attributeValue = sb.ToString();

                                if (attributeRenderer != null)
                                {
                                    attributeValue = attributeRenderer.PostRender(statement.Attributes[i].Key, attributeValue);
                                }

                                switch (attributeValue)
                                {
                                    case "true":
                                        statement.Attributes[i] = new Parrot.Nodes.Attribute(Host, statement.Attributes[i].Key, new StringLiteral(Host, "\"checked\""));
                                        //.Value = "checked";
                                        break;
                                    case "false":
                                    case "null":
                                        //remove this attribute
                                        statement.Attributes.RemoveAt(i);
                                        i -= 1;
                                        break;
                                }
                            }
                        }
                    }

                    break;
            }

            base.Render(writer, statement, documentHost, model);
        }
    }
}