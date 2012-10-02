using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Parrot.Infrastructure;
using Parrot.Nodes;
using Parrot.Renderers.Infrastructure;

namespace Parrot.Mvc.Renderers
{
    public class DocumentRenderer : Parrot.Renderers.DocumentRenderer
    {
        public DocumentRenderer(IHost host) : base(host) { }

        public string RenderFromLayout(Document document, StatementList children, object model)
        {
            var factory = Host.DependencyResolver.Resolve<IRendererFactory>();

            StringBuilder sb = new StringBuilder();
            foreach (var element in document.Children)
            {
                var renderer = factory.GetRenderer(element.Name);

                if (renderer is ContentRenderer)
                {
                    sb.AppendLine((renderer as ContentRenderer).RenderFromLayout(element, children, model));
                }
                else
                {
                    sb.AppendLine(renderer.Render(element, model));
                }
            }

            return sb.ToString().Trim();
        }

        public override string Render(Document document, object model)
        {
            var factory = Host.DependencyResolver.Resolve<IRendererFactory>();

            StringBuilder sb = new StringBuilder();
            foreach (var element in document.Children)
            {
                var renderer = factory.GetRenderer(element.Name);

                sb.AppendLine(renderer.Render(element, model));
            }

            return sb.ToString().Trim();
        }

        public override string Render(StatementList statements, object model)
        {
            var factory = Host.DependencyResolver.Resolve<IRendererFactory>();

            StringBuilder sb = new StringBuilder();
            foreach (var element in statements)
            {
                var renderer = factory.GetRenderer(element.Name);

                sb.AppendLine(renderer.Render(element, model));
            }

            return sb.ToString().Trim();
        }

    }
}
