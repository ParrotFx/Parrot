// -----------------------------------------------------------------------
// <copyright file="UlRenderer.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using Parrot.Infrastructure;

namespace Parrot.Mvc.Renderers
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class UlRenderer : HtmlRenderer
    {
        public UlRenderer(IHost host) : base(host) {}

        public override string DefaultChildTag
        {
            get { return "li"; }
        }
    }
}
