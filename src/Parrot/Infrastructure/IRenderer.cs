using Parrot.Nodes;

namespace Parrot.Infrastructure
{
    public interface IRenderer
    {
        string Render(AbstractNode node, object model);
        string Render(AbstractNode node);
    }
}