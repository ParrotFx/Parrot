namespace Parrot.Mvc.Renderers
{
    using System;
    using System.Web;
    using System.Web.Mvc;
    using Parrot.Renderers.Infrastructure;

    internal class AttributeRenderer : IAttributeRenderer
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