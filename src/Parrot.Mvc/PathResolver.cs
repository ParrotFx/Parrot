// -----------------------------------------------------------------------
// <copyright file="PathResolver.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------


namespace Parrot.Mvc
{
    using System;
    using System.IO;
    using System.Web;
    using System.Web.Hosting;
    using System.Web.Mvc;
    using Parrot.Renderers.Infrastructure;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class PathResolver : IPathResolver
    {
        public Stream OpenFile(string path)
        {
            return VirtualPathProvider.OpenFile(path);
        }

        public bool FileExists(string path)
        {
            return HostingEnvironment.VirtualPathProvider.FileExists(path);
        }

        public string ResolvePath(string path)
        {
            if (FileExists(path))
            {
                //add it to cache - not currently implemented
                return HostingEnvironment.VirtualPathProvider.GetFile(path).VirtualPath;
            }

            throw new FileNotFoundException(path);
        }

        public string ResolveAttributeRelativePath(string key, object value)
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