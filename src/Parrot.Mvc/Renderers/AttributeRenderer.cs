using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Parrot.Renderers;
using Parrot.Renderers.Infrastructure;

namespace Parrot.Mvc.Renderers
{
    class AttributeRenderer : IAttributeRenderer
    {
        public string PostRender(string key, object value)
        {
            if (value != null)
            {
                string temp = value.ToString();
                if (temp.StartsWith("~/") && !key.StartsWith("data-val", StringComparison.OrdinalIgnoreCase))
                {
                    //convert this to a server path

                    return UrlHelper.GenerateContentUrl(temp, new HttpContextWrapper(HttpContext.Current));
                }

                return temp;
            }

            return null;

        }
    }
}
