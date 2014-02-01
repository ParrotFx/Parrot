namespace Parrot.Tests.Parser
{
    using System.Collections.Generic;
    using Infrastructure;
    using Parrot.Nodes;
    using Renderers;
    using Renderers.Infrastructure;

    public class ParrotParserTestsBase
    {
        public static Document Parse(string text)
        {
            Parrot.Parser.Parser parser = new Parrot.Parser.Parser();
            Document document;

            parser.Parse(text, out document);

            return document;
        }

        public string Render(Document document)
        {
            var host = new SimpleHost();
            var rf = host.RendererFactory;
            var documentHost = new Dictionary<string, object>();

            DocumentView view = new DocumentView(host, rf, documentHost, document);

            IParrotWriter writer = host.CreateWriter();
            view.Render(writer);

            return writer.Result();
        }

        private sealed class SimpleHost : IHost
        {
            public SimpleHost()
            {
                ValueTypeProvider = new ValueTypeProvider();
                ModelValueProviderFactory = new ModelValueProviderFactory(ValueTypeProvider);
                RendererFactory = new RendererFactory(new IRenderer[]
                {
                    new HtmlRenderer(this),
                    new StringLiteralRenderer(this),
                    new DocTypeRenderer(this),
                    new ForeachRenderer(this),
                    new InputRenderer(this),
                    new ConditionalRenderer(this),
                    new ListRenderer(this),
                    new SelfClosingRenderer(this),
                });
            }

            public IModelValueProviderFactory ModelValueProviderFactory { get; set; }
            public IAttributeRenderer AttributeRenderer { get; set; }
            public IValueTypeProvider ValueTypeProvider { get; set; }
            public IRendererFactory RendererFactory { get; set; }
            public IPathResolver PathResolver { get; set; }

            public IParrotWriter CreateWriter()
            {
                return new PrettyStringWriter();
            }
        }

    }
}