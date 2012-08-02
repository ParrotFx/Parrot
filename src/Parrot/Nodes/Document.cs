// -----------------------------------------------------------------------
// <copyright file="Document.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Parrot.Nodes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Document
    {
        public BlockNodeList Children;

        public Document()
        {
            Children = new BlockNodeList();
        }

        public string Render(object model)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var element in Children)
            {
                var factory = Infrastructure.Host.DependencyResolver.Get<IRendererFactory>();

                var renderer = factory.GetRenderer(element.BlockName);
                sb.AppendLine(renderer.Render(element, model));
            }

            return sb.ToString();
        }
    }
}