namespace Parrot.Renderers.Infrastructure
{
    public interface IAttributeRenderer
    {
        string PostRender(string key, object value);
    }
}