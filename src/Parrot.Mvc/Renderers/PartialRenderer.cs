using System.Collections.Generic;
using Parrot.Infrastructure;

namespace Parrot.Mvc.Renderers
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Web.Mvc;
    using Nodes;
    using Parrot.Renderers;
    using Parrot.Renderers.Infrastructure;

    public class PartialRenderer : HtmlRenderer
    {
        public PartialRenderer(IHost host) : base(host)
        {
            
        }

        public override void Render(IParrotWriter writer, IRendererFactory rendererFactory, Statement statement, IDictionary<string, object> documentHost, object model)
        {
            object localModel = documentHost;

            //if (blockNode.Parameters != null && blockNode.Parameters.Any())
            //{
            //    localModel = modelValueProviderFactory.Get(model.GetType()).GetValue(model, blockNode.Parameters.First().ValueType,
            //                                               blockNode.Parameters.First().Value);
            //}

            //get the parameter
            string layout = "";
            if (statement.Parameters != null && statement.Parameters.Any())
            {
                //assume only the first is the path
                //second is the argument (model)
                layout = statement.Parameters[0].Value;
            }

            //ok...we need to load the layoutpage
            //then pass the node's children into the layout page
            //then return the result
            var engine = Host.DependencyResolver.Resolve<IViewEngine>();
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

        //public override string Render(AbstractNode node, object documentHost)
        //{
        //    if (_engine == null)
        //    {
        //        _engine = Host.DependencyResolver.Resolve<IViewEngine>();
        //    }

        //    var modelValueProviderFactory = Host.DependencyResolver.Resolve<IModelValueProviderFactory>();

        //    if (node == null)
        //    {
        //        throw new ArgumentNullException("node");
        //    }

        //    var blockNode = node as Statement;
        //    if (blockNode == null)
        //    {
        //        throw new InvalidCastException("node");
        //    }

        //    object localModel = documentHost;

        //    //if (blockNode.Parameters != null && blockNode.Parameters.Any())
        //    //{
        //    //    localModel = modelValueProviderFactory.Get(model.GetType()).GetValue(model, blockNode.Parameters.First().ValueType,
        //    //                                               blockNode.Parameters.First().Value);
        //    //}

        //    //get the parameter
        //    string layout = "";
        //    if (blockNode.Parameters != null && blockNode.Parameters.Any())
        //    {
        //        //assume only the first is the path
        //        //second is the argument (model)
        //        layout = blockNode.Parameters[0].Value;
        //    }

        //    //ok...we need to load the layoutpage
        //    //then pass the node's children into the layout page
        //    //then return the result
        //    var result = _engine.FindView(null, layout, null, false);
        //    if (result != null)
        //    {
        //        var parrotView = (result.View as ParrotView);
        //        using (var stream = parrotView.LoadStream())
        //        {
        //            string contents = new StreamReader(stream).ReadToEnd();

        //            var document = parrotView.LoadDocument(contents);

        //            return Host.DependencyResolver.Resolve<DocumentRenderer>().Render(document, localModel);

        //        }
        //    }

        //    throw new InvalidOperationException();
        //}
    }
}