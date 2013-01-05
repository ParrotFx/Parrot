// -----------------------------------------------------------------------
// <copyright file="LayoutRenderer.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Parrot.Mvc.Renderers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Parrot.Infrastructure;
    using Parrot.Nodes;
    using Parrot.Renderers;
    using Parrot.Renderers.Infrastructure;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LayoutRenderer : HtmlRenderer
    {
        public LayoutRenderer(IHost host) : base(host)
        {
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
                Type modelType = model != null ? model.GetType() : null;
                var modelValueProvider = Host.ModelValueProviderFactory.Get(modelType);

                var parameterLayout = GetLocalModelValue(documentHost, statement, modelValueProvider, model) ?? "_layout";

                //assume only the first is the path
                //second is the argument (model)
                layout = parameterLayout.ToString();
            }


            //ok...we need to load the view
            //then pass the model to it and
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

                    //Create a new DocumentView and DocumentHost
                    if (!documentHost.ContainsKey("_LayoutChildren_"))
                    {
                        documentHost.Add("_LayoutChildren_", new Queue<StatementList>());
                    }
                    (documentHost["_LayoutChildren_"] as Queue<StatementList>).Enqueue(statement.Children);

                    DocumentView view = new DocumentView(Host, rendererFactory, documentHost, document);

                    view.Render(writer);
                }
            }
        }
    }
}