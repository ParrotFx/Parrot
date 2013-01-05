namespace Parrot.Renderers.Infrastructure
{
    using System.Collections.Generic;
    using Parrot.Infrastructure;

    public interface IModelValueProvider
    {
        bool GetValue(IDictionary<string, object> documentHost, object model, object property, out object value);
        bool GetValue(IDictionary<string, object> documentHost, object model, ValueType valueType, object property, out object value);
    }
}