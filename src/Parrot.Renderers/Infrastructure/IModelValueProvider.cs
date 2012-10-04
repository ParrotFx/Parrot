namespace Parrot.Renderers.Infrastructure
{
    public interface IModelValueProvider
    {
        bool GetValue(object model, Parrot.Infrastructure.ValueType valueType, object property, out object value);
    }
}