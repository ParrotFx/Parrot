namespace Parrot.Renderers.Infrastructure
{
    using System.IO;

    public interface IPathResolver
    {
        Stream OpenFile(string path);
        bool FileExists(string path);
        string ResolvePath(string path);
        string ResolveAttributeRelativePath(string key, object value);
    }
}