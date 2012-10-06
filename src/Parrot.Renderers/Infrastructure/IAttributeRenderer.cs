using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parrot.Renderers.Infrastructure
{
    public interface IAttributeRenderer
    {
        string PostRender(string key, object value);
    }
}
