namespace Parrot.Mvc
{
    using Parrot.Infrastructure;

    public class StandardWriterProvider : IParrotWriterProvider
    {
        public IParrotWriter CreateWriter()
        {
            return new StandardWriter();
        }
    }
}