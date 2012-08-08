// -----------------------------------------------------------------------
// <copyright file="RendererHelpers.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Parrot.Mvc.Renderers
{
    using System.Collections.Generic;
    using System.Text;
    using Nodes;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class RendererHelpers
    {
        public static string Render(this IList<Statement> nodes, object model )
        {
            var factory = Infrastructure.Host.DependencyResolver.Get<IRendererFactory>();
            StringBuilder sb = new StringBuilder();
            foreach (var child in nodes)
            {
                sb.Append(factory.GetRenderer(child.Name).Render(child, model));
            }

            return sb.ToString();
        }
    }
}
