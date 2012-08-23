// -----------------------------------------------------------------------
// <copyright file="LayoutRenderer.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

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
    public class LayoutRenderer : IRenderer
    {
        private readonly IHost _host;

        public LayoutRenderer(IHost host)
        {
            _host = host;
        }
        
        public string Render(AbstractNode node, object model)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            var blockNode = node as Statement;
            if (blockNode == null)
            {
                throw new InvalidCastException("node");
            }

            //get the parameter
            string layout = "";
            if (blockNode.Parameters != null && blockNode.Parameters.Any())
            {
                //assume only the first is the path
                //second is the argument (model)
                layout = blockNode.Parameters[0].Value;
            }

            //ok...we need to load the view
            //then pass the model to it and
            //then return the result
            var engine = _host.DependencyResolver.Get<IViewEngine>();
            var result = engine.FindView(null, layout, null, false);
            if (result != null)
            {
                var parrotView = (result.View as ParrotView);
                using (var stream = parrotView.LoadStream())
                {
                    string contents = new StreamReader(stream).ReadToEnd();

                    var document = parrotView.LoadDocument(contents);
                    var renderer = _host.DependencyResolver.Get<DocumentRenderer>();

                    return renderer.Render(document, new
                    {
                        Children = new StatementList(_host, blockNode.Children.ToArray()),
                        Model = model
                    });
                }
            }

            throw new InvalidOperationException();
        }

        public string Render(AbstractNode node)
        {
            return Render(node, null);
        }
    }
}
