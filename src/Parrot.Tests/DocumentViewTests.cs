// -----------------------------------------------------------------------
// <copyright file="DocumentViewTests.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Parrot.Tests
{
    using System.Collections.Generic;
    using NUnit.Framework;
    using Parrot.Nodes;
    using Parrot.Renderers;
    using Parrot.Renderers.Infrastructure;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class DocumentViewTests
    {
        [Test]
        public void Blah()
        {
            var text = "div[attr1='test'](Model) > @Value";
            var host = new MemoryHost();

            var parser = new Parser.Parser();
            var documentHost = new Dictionary<string, object>
                {
                    {"Model", new {Value = "This is a test"}}
                };

            Document document;
            parser.Parse(text, out document);

            var rendererFactory = new RendererFactory(new IRenderer[] {new HtmlRenderer(host), new StringLiteralRenderer(host)});

            DocumentView documentView = new DocumentView(
                host,
                rendererFactory,
                documentHost,
                document
                );

            var writer = host.CreateWriter();

            documentView.Render(writer);

            string result = writer.Result();

            Assert.AreEqual("<div attr1=\"test\">This is a test</div>", result);
        }
    }
}