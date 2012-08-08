// -----------------------------------------------------------------------
// <copyright file="RendererHelpers.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using Parrot.Infrastructure;

namespace Parrot.Mvc.Renderers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Nodes;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class RendererHelpers
    {
        public static string Render(this IList<Statement> nodes, object model, LocalsStack stack)
        {
            var factory = Infrastructure.Host.DependencyResolver.Get<IRendererFactory>();
            StringBuilder sb = new StringBuilder();
            foreach (var child in nodes)
            {
                sb.Append(factory.GetRenderer(child.Name).Render(child, model, stack));
            }

            return sb.ToString();
        }
    }
}
