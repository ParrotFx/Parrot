namespace Parrot.Renderers.Infrastructure
{
    using System;

    public interface IModelValueProviderFactory
    {
        void Register(Type type, Func<IModelValueProvider> provider);
        IModelValueProvider Get(Type type);
    }
}