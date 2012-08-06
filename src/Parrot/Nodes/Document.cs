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
        public StatementList Children;

        public Document()
        {
            Children = new StatementList();
        }

        public Document(Document document, Statement statement)
        {
            this.Children = document.Children;
            this.Children.Add(statement);
        }

        public string Render(object model)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var element in Children)
            {
                var factory = Infrastructure.Host.DependencyResolver.Get<IRendererFactory>();

                var renderer = factory.GetRenderer(element.Name);
                sb.AppendLine(renderer.Render(element, model));
            }

            return sb.ToString();
        }
    }
}