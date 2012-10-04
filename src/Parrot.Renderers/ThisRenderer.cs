// -----------------------------------------------------------------------
// <copyright file="ThisRenderer.cs" company="">
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
    public class ThisRenderer : IRenderer
    {

        private readonly IHost _host;
        public ThisRenderer(IHost host)
        {
            _host = host;
        }

        public string Render(AbstractNode node, object documentHost)
        {
            return documentHost.ToString();
        }

        public string Render(AbstractNode node)
        {
            return "null"; //throw a null reference exception here?
        }
    }
}
