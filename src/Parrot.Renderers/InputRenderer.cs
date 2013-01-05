namespace Parrot.Renderers
{
    using System;
    using System.Collections.Generic;
    using Parrot.Infrastructure;
    using Parrot.Nodes;
    using Parrot.Renderers.Infrastructure;

    public class InputRenderer : SelfClosingRenderer
    {
        public InputRenderer(IHost host) : base(host)
        {
        }

        public override IEnumerable<string> Elements
        {
            get { yield return "input"; }
        }

        private string GetType(Statement statement, IRendererFactory rendererFactory, IDictionary<string, object> documentHost, object model)
        {
            for (int i = 0; i < statement.Attributes.Count; i++)
            {
                if (statement.Attributes[i].Key.Equals("type", StringComparison.OrdinalIgnoreCase))
                {
                    return RenderAttribute(statement.Attributes[i], rendererFactory, documentHost, model);
                }
            }

            return "hidden";
        }

        public override void Render(IParrotWriter writer, IRendererFactory rendererFactory, Statement statement, IDictionary<string, object> documentHost, object model)
        {
            //get the input type
            string type = GetType(statement, rendererFactory, documentHost, model);
            switch (type)
            {
                case "checkbox":
                case "radio":
                    //special handling for "checked"
                    //is there a checked attribute
                    for (int i = 0; i < statement.Attributes.Count; i++)
                    {
                        if (statement.Attributes[i].Key == "checked")
                        {
                            string attributeValue = RenderAttribute(statement.Attributes[i], rendererFactory, documentHost, model);

                            switch (attributeValue)
                            {
                                case "true":
                                    statement.Attributes[i] = new Nodes.Attribute(statement.Attributes[i].Key, new StringLiteral("\"checked\""));
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

                    break;
            }

            base.Render(writer, rendererFactory, statement, documentHost, model);
        }
    }
}