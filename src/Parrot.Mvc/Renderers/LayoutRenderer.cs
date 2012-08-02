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
    using System.Text;
    using Nodes;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LayoutRenderer : IRenderer
    {
        private ParrotViewEngine engine;

        public LayoutRenderer()
        {
            engine = new ParrotViewEngine();
        }

        public string Render(AbstractNode node, object model)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            var blockNode = node as BlockNode;
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

            //ok...we need to load the layoutpage
            //then pass the node's children into the layout page
            //then return the result
            var result = engine.FindView(null, layout, null, false);
            if (result != null)
            {
                var parrotView = (result.View as ParrotView);
                using (var stream = parrotView.LoadStream())
                {
                    string contents = new StreamReader(stream).ReadToEnd();

                    var document = ParrotView.LoadDocument(contents);

                    return document.Render(new
                    {
                        Children = new BlockNodeList(blockNode.Children.ToArray()),
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

    public class ContentRenderer : IRenderer
    {
        public string Render(AbstractNode node, object model)
        {
            dynamic localModel = model;

            Document document = new Document
            {
                Children = localModel.Children
            };

            return document.Render(localModel.Model);
        }



        [Obsolete]
        public string Render(AbstractNode node)
        {
            throw new InvalidOperationException();
        }
    }
}
