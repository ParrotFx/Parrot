// -----------------------------------------------------------------------
// <copyright file="LiteralRenderer.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using Parrot.Infrastructure;
using Parrot.Nodes;
using Parrot.Renderers.Infrastructure;

namespace Parrot.Renderers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LiteralRenderer : IRenderer
    {
        private readonly IHost _host;

        public LiteralRenderer(IHost host)
        {
            _host = host;
        }

        public string Render(AbstractNode node, object documentHost)
        {
            if (documentHost != null)
            {
                var modelValueProviderFactory = _host.DependencyResolver.Resolve<IModelValueProviderFactory>();

                var value = modelValueProviderFactory.Get(documentHost.GetType()).GetValue(documentHost, Parrot.Infrastructure.ValueType.Property, (node as Statement).Name);

                return value.ToString();
            }
            return (node as Statement).Name;
        }

        public string Render(AbstractNode node)
        {
            return Render(node, null);
        }
    }
}