// -----------------------------------------------------------------------
// <copyright file="BaseRenderer.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Parrot.Renderers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>

    public abstract class BaseRenderer<T>
    {
        protected T RenderingHost;

        //needs a way to override property resolution
    }

    public class ModelRenderingHost: IRenderingHost
    {
        public object Model { get; set; }
    }

    public interface IRenderingHost { }
}