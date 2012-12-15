// -----------------------------------------------------------------------
// <copyright file="ListRenderer.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;

namespace Parrot.Renderers
{
    using Parrot.Infrastructure;
    using Parrot.Renderers.Infrastructure;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ListRenderer : HtmlRenderer
    {
        public ListRenderer(IHost host) : base(host) { }

        public override string DefaultChildTag
        {
            get { return "li"; }
        }
    }
}
