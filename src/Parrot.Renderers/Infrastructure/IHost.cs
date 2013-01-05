namespace Parrot.Renderers.Infrastructure
{
    using Parrot.Infrastructure;

    public interface IHost
    {
        IModelValueProviderFactory ModelValueProviderFactory { get; set; }
        IRendererFactory RendererFactory { get; set; }
        IPathResolver PathResolver { get; set; }
        IParrotWriter CreateWriter();
    }
}