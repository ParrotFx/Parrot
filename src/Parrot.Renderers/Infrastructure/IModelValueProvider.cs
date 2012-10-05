using System.Collections.Generic;

namespace Parrot.Renderers.Infrastructure
{
    public interface IModelValueProvider
    {
        bool GetValue(IDictionary<string, object> documentHost, object model, Parrot.Infrastructure.ValueType valueType, object property, out object value);
    }
}