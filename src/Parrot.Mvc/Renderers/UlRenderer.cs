﻿// -----------------------------------------------------------------------
// <copyright file="UlRenderer.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Parrot.Mvc.Renderers
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class UlRenderer : HtmlRenderer
    {
        public override string DefaultChildTag
        {
            get { return "li"; }
        }
    }
}
