// -----------------------------------------------------------------------
// <copyright file="ListRenderer.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Parrot.Renderers
{
    using System.Collections.Generic;
    using Parrot.Renderers.Infrastructure;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ListRenderer : HtmlRenderer
    {
        public ListRenderer(IHost host) : base(host)
        {
        }

        public override IEnumerable<string> Elements
        {
            get
            {
                yield return "ul";
                yield return "ol";
            }
        }

        public override string DefaultChildTag
        {
            get { return "li"; }
        }
    }
}