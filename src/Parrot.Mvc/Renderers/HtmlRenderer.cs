using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Parrot.Infrastructure;

namespace Parrot.Mvc.Renderers
{
    public class HtmlRenderer : Parrot.Renderers.HtmlRenderer
    {
        public HtmlRenderer(IHost host) : base(host)
        {
            PreRenderAttribute = (key, value) => GenerateContentUrl(value);
        }

        internal static string GenerateContentUrl(object value)
        {
            if (value != null)
            {
                string temp = value.ToString();
                if (temp.StartsWith("~/") && !temp.StartsWith("data-val", StringComparison.OrdinalIgnoreCase))
                {
                    //convert this to a server path

                    return UrlHelper.GenerateContentUrl(temp, new HttpContextWrapper(System.Web.HttpContext.Current));
                }

                return temp;
            }

            return null;
        }

    }
}
