namespace Parrot.Mvc.Renderers
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Web.Mvc;
    using Nodes;

    public class PartialRenderer : HtmlRenderer
    {
        private readonly ParrotViewEngine _engine;

        public PartialRenderer(IViewEngine engine)
        {
            _engine = new ParrotViewEngine();
        }

        public PartialRenderer() : this(new ParrotViewEngine()) { }

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

            object localModel = model;

            if (blockNode.Parameters != null && blockNode.Parameters.Any())
            {
                blockNode.Parameters.First().SetModel(model);

                localModel = (blockNode.Parameters.First() as Parameter).GetPropertyValue();
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