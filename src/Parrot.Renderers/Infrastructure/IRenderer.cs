using Parrot.Nodes;

namespace Parrot.Renderers.Infrastructure
{
    public interface IRenderer
    {
        string Render(AbstractNode node, object model);
        string Render(AbstractNode node);
    }
}