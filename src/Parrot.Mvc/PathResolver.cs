// -----------------------------------------------------------------------
// <copyright file="PathResolver.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------


namespace Parrot.Mvc
{
    using System.IO;
    using System.Web.Hosting;

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

        public string VirtualFilePath(string path)
        {
            if (FileExists(path))
            {
                //add it to cache - not currently implemented
                return HostingEnvironment.VirtualPathProvider.GetFile(path).VirtualPath;
            }

            throw new FileNotFoundException(path);
        }
    }

    public interface IPathResolver
    {
        Stream OpenFile(string path);
        bool FileExists(string path);
        string VirtualFilePath(string path);
    }
}
