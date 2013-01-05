namespace Parrot.Mvc.Renderers
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Parrot.Infrastructure;
    using Parrot.Nodes;
    using Parrot.Renderers;
    using Parrot.Renderers.Infrastructure;

    public class PartialRenderer : HtmlRenderer
    {
        public PartialRenderer(IHost host) : base(host)
        {
        }

        public override IEnumerable<string> Elements
        {
            get { yield return "partial"; }
        }

        public override void Render(IParrotWriter writer, IRendererFactory rendererFactory, Statement statement, IDictionary<string, object> documentHost, object model)
        {
            //get the parameter
            string layout;
            if (statement.Parameters != null && statement.Parameters.Any())
            {
                //assume only the first is the path
                //second is the argument (model)
                layout = statement.Parameters[0].Value;
            }
            else
            {
                layout = "_layout";
            }

            //ok...we need to load the layoutpage
            //then pass the node's children into the layout page
            //then return the result
            var engine = (Host as AspNetHost).ViewEngine;
            var result = engine.FindView(null, layout, null, false);
            if (result != null)
            {
                var parrotView = (result.View as ParrotView);
                using (var stream = parrotView.LoadStream())
                {
                    string contents = new StreamReader(stream).ReadToEnd();

                    var document = parrotView.LoadDocument(contents);

                    DocumentView view = new DocumentView(Host, rendererFactory, documentHost, document);

                    view.Render(writer);
                }
            }
        }
    }
}