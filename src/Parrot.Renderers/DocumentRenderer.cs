// -----------------------------------------------------------------------
// <copyright file="DocumentRenderer.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using Parrot.Renderers.Infrastructure;
using Parrot.Nodes;

namespace Parrot.Renderers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Parrot.Infrastructure;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class DocumentRenderer
    {
        protected readonly IHost Host;

        public DocumentRenderer(IHost host)
        {
            Host = host;
        }

        public virtual string Render(Document document, object model)
        {
            var factory = Host.DependencyResolver.Resolve<IRendererFactory>();

            StringBuilder sb = new StringBuilder();
            foreach (var element in document.Children)
            {
                var renderer = factory.GetRenderer(element.Name);
                sb.AppendLine(renderer.Render(element, model));
            }

            return sb.ToString().Trim();
        }

        public virtual string Render(StatementList statements, object model)
        {
            var factory = Host.DependencyResolver.Resolve<IRendererFactory>();

            StringBuilder sb = new StringBuilder();
            foreach (var element in statements)
            {
                var renderer = factory.GetRenderer(element.Name);
                sb.AppendLine(renderer.Render(element, model));
            }

            return sb.ToString().Trim();
        }
    }
}
