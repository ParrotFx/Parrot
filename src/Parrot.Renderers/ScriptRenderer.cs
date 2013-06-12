using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parrot.Renderers
{
    using Parrot.Renderers.Infrastructure;

    public class ScriptRenderer : HtmlRenderer
    {
        public override IEnumerable<string> Elements { get { yield return "script"; } }
        
        public ScriptRenderer(IHost host) : base(host) { }


        protected override void CreateTag(Parrot.Infrastructure.IParrotWriter writer, IRendererFactory rendererFactory, IDictionary<string, object> documentHost, object model, Nodes.Statement statement)
        {
            var xhtml = false;
            if (documentHost.ContainsKey("doctype"))
            {
                //we have a registered doctype, is it xml?
                if (documentHost["doctype"].ToString().IndexOf("xhtml", StringComparison.OrdinalIgnoreCase) != -1)
                {
                    //it's xhtml, we need to output cdata
                    xhtml = true;
                }
            }


            string tagName = string.IsNullOrWhiteSpace(statement.Name) ? DefaultChildTag : statement.Name;

            TagBuilder builder = new TagBuilder(tagName);
            //add attributes
            RenderAttributes(rendererFactory, documentHost, model, statement, builder);
            //AppendAttributes(builder, statement.Attributes, documentHost, modelValueProvider);

            writer.Write(builder.ToString(TagRenderMode.StartTag));
            //render children

            if (xhtml)
            {
                writer.Write("//<![CDATA[");
            }

            if (statement.Children.Count > 0)
            {
                RenderChildren(writer, statement, rendererFactory, documentHost, model);
            }

            if (xhtml)
            {
                writer.Write("//]]>");
            }

            writer.Write(builder.ToString(TagRenderMode.EndTag));
        }
    }
}
