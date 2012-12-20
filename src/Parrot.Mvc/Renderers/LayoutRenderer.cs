// -----------------------------------------------------------------------
// <copyright file="LayoutRenderer.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Web.Mvc;
using Parrot.Infrastructure;

namespace Parrot.Mvc.Renderers
{
    using System;
    using System.IO;
    using System.Linq;
    using Nodes;
    using Parrot.Renderers;
    using Parrot.Renderers.Infrastructure;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LayoutRenderer : HtmlRenderer, IRenderer
    {
        private readonly IHost _host;

        public LayoutRenderer(IHost host) : base(host)
        {
            _host = host;
        }

        public override IEnumerable<string> Elements
        {
            get { yield return "layout"; }
        }

        public override void Render(IParrotWriter writer, IRendererFactory rendererFactory, Statement statement, IDictionary<string, object> documentHost, object model)
        {
            string layout = "";
            if (statement.Parameters != null && statement.Parameters.Any())
            {
                //assume only the first is the path
                //second is the argument (model)
                layout = statement.Parameters[0].Value;
            }

            //ok...we need to load the view
            //then pass the model to it and
            //then return the result
            var engine = _host.DependencyResolver.Resolve<IViewEngine>();
            var result = engine.FindView(null, layout, null, false);
            if (result != null)
            {
                var parrotView = (result.View as ParrotView);
                using (var stream = parrotView.LoadStream())
                {
                    string contents = new StreamReader(stream).ReadToEnd();

                    var document = parrotView.LoadDocument(contents);

                    //Create a new DocumentView and DocumentHost
                    if (!documentHost.ContainsKey("_LayoutChildren_"))
                    {
                        documentHost.Add("_LayoutChildren_", new Queue<StatementList>());
                    }
                    (documentHost["_LayoutChildren_"] as Queue<StatementList>).Enqueue(statement.Children);

                    DocumentView view = new DocumentView(_host, rendererFactory, documentHost, document);

                    view.Render(writer);
                }
            }
        }
    }
}
