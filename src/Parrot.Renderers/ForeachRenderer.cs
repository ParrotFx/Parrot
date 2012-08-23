// -----------------------------------------------------------------------
// <copyright file="ForeachRenderer.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using Parrot.Renderers.Infrastructure;

namespace Parrot.Renderers
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Text;
    using Nodes;
    using Parrot.Infrastructure;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ForeachRenderer : HtmlRenderer
    {
        public ForeachRenderer(IHost host) : base(host) {}

        public override string Render(AbstractNode node, object model)
        {
            var modelValueProviderFactory = Host.DependencyResolver.Get<IModelValueProviderFactory>();
            object localModel = model;
            
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            var blockNode = node as Statement;
            if (blockNode == null)
            {
                //somehow we're not rendering a blockNode
                throw new InvalidCastException("node");
            }

            //use the passed in parameter property or use the page model
            if (blockNode.Parameters.Any())
            {
                localModel = modelValueProviderFactory.Get(model.GetType()).GetValue(model, blockNode.Parameters[0].ValueType, blockNode.Parameters[0].Value);
            }

            //Assert that we're looping over something
            IEnumerable loop = localModel as IEnumerable;
            if (loop == null)
            {
                throw new InvalidCastException("model is not IEnumerable");
            }

            StringBuilder sb = new StringBuilder();
            var documentRenderer = Host.DependencyResolver.Get<DocumentRenderer>();
            foreach (var item in loop)
            {
                sb.Append(documentRenderer.Render(blockNode.Children, item));
                //sb.Append(blockNode.Children.Render(item));
            }

            return sb.ToString();
        }

        public override string Render(AbstractNode node)
        {
            return Render(node, null);
        }
    }
}
