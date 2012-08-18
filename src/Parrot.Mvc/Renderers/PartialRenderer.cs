namespace Parrot.Mvc.Renderers
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Web.Mvc;
    using Nodes;

    public class PartialRenderer : HtmlRenderer
    {
        private readonly IViewEngine _engine;

        public PartialRenderer(IViewEngine engine)
        {
            _engine = engine;
        }

        public PartialRenderer() : this(new ParrotViewEngine(Parrot.Infrastructure.Host.DependencyResolver.Get<IPathResolver>())) { }

        public override string Render(AbstractNode node, object model)
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

            object localModel = model;

            if (blockNode.Parameters != null && blockNode.Parameters.Any())
            {
                localModel = RendererHelpers.GetModelValue(model, blockNode.Parameters.First().ValueType,
                                                           blockNode.Parameters.First().Value);
            }

            //get the parameter
            string layout = "";
            if (blockNode.Parameters != null && blockNode.Parameters.Any())
            {
                //assume only the first is the path
                //second is the argument (model)
                layout = blockNode.Parameters[0].Value;
            }

            //ok...we need to load the layoutpage
            //then pass the node's children into the layout page
            //then return the result
            var result = _engine.FindView(null, layout, null, false);
            if (result != null)
            {
                var parrotView = (result.View as ParrotView);
                using (var stream = parrotView.LoadStream())
                {
                    string contents = new StreamReader(stream).ReadToEnd();

                    var document = ParrotView.LoadDocument(contents);

                    return document.Render(localModel);
                }
            }

            throw new InvalidOperationException();
        }
    }
}