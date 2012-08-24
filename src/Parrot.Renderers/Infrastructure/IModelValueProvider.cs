namespace Parrot.Renderers.Infrastructure
{
    public interface IModelValueProvider
    {
        object GetValue(object model, Parrot.Infrastructure.ValueType valueType, object property);
    }
}