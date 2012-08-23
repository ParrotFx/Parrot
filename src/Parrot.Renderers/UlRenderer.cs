// -----------------------------------------------------------------------
// <copyright file="UlRenderer.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Parrot.Renderers
{
    using Parrot.Infrastructure;

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
