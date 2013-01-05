namespace Parrot.Mvc
{
    using Parrot.Infrastructure;

    public interface IParrotWriterProvider
    {
        IParrotWriter CreateWriter();
    }
}