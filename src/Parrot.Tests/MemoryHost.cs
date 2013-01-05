// -----------------------------------------------------------------------
// <copyright file="MemoryHost.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Parrot.Tests
{
    using Parrot.Infrastructure;
    using Parrot.Mvc;
    using Parrot.Renderers.Infrastructure;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MemoryHost : AspNetHost
    {
        public MemoryHost() : base(new StandardWriterProvider())
        {
        }

        public MemoryHost(IParrotWriterProvider parrotWriterProvider) : base(parrotWriterProvider)
        {
            IValueTypeProvider valueTypeProvider = new ValueTypeProvider();
            ModelValueProviderFactory = new ModelValueProviderFactory(valueTypeProvider);
        }

        public override IParrotWriter CreateWriter()
        {
            return new StandardWriter();
        }
    }

    public class StandardWriterProvider : IParrotWriterProvider
    {
        public IParrotWriter CreateWriter()
        {
            return new StandardWriter();
        }
    }
}