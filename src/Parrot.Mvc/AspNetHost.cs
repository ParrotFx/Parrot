// -----------------------------------------------------------------------
// <copyright file="AspNetHost.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------


namespace Parrot.Mvc
{
    using System.Web.Mvc;
    using Parrot.Infrastructure;
    using Parrot.Mvc.Renderers;
    using Parrot.Renderers;
    using Parrot.Renderers.Infrastructure;

    /// <summary>
    /// Asp.Net host for Parrot
    /// </summary>
    public class AspNetHost : IHost
    {
        private readonly IParrotWriterProvider _parrotWriterProvider;

        public AspNetHost() : this(new StandardWriterProvider())
        {
        }

        public AspNetHost(IParrotWriterProvider parrotWriterProvider)
        {
            _parrotWriterProvider = parrotWriterProvider;
            ValueTypeProvider = new ValueTypeProvider();
            ModelValueProviderFactory = new ModelValueProviderFactory(ValueTypeProvider);
            AttributeRenderer = new AttributeRenderer();
            RendererFactory = new RendererFactory(new IRenderer[]
                {
                    new HtmlRenderer(this),
                    new StringLiteralRenderer(this),
                    new DocTypeRenderer(this),
                    new LayoutRenderer(this),
                    new PartialRenderer(this),
                    new ContentRenderer(this),
                    new ForeachRenderer(this),
                    new InputRenderer(this),
                    new ConditionalRenderer(this),
                    new ListRenderer(this),
                    new SelfClosingRenderer(this)
                });
            PathResolver = new PathResolver();
        }

        public IModelValueProviderFactory ModelValueProviderFactory { get; set; }
        public IAttributeRenderer AttributeRenderer { get; set; }
        public IValueTypeProvider ValueTypeProvider { get; set; }
        public IRendererFactory RendererFactory { get; set; }
        public IPathResolver PathResolver { get; set; }
        public IViewEngine ViewEngine { get; set; }

        public virtual IParrotWriter CreateWriter()
        {
            return _parrotWriterProvider.CreateWriter();
        }
    }
}