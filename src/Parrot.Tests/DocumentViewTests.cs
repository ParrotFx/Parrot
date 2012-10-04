// -----------------------------------------------------------------------
// <copyright file="DocumentViewTests.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System.IO;
using NUnit.Framework;
using Parrot.Nodes;
using Parrot.Renderers;
using Parrot.Renderers.Infrastructure;

namespace Parrot.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class DocumentViewTests
    {
        [Test]
        public void Blah()
        {
            var text = "div[attr1='test'](Model) > :Value";
            var host = new MemoryHost();

            var parser = new Parser.Parser(host);
            var documentHost = new Dictionary<string, object>
            {
                { "Model" , new { Value = "This is a test" } } 
            };

            Document document;
            parser.Parse(text, out document);
            
            var rendererFactory = new RendererFactory(host);
            rendererFactory.RegisterFactory("*", new HtmlRenderer(host, rendererFactory));
            rendererFactory.RegisterFactory("string", new StringLiteralRenderer(host, rendererFactory));

            DocumentView documentView = new DocumentView(
                host,
                rendererFactory,
                documentHost,
                document
            );

            var sb = new StringBuilder();
            var writer = new StringWriter(sb);

            documentView.Render(writer);

            string result = sb.ToString();

            Assert.AreEqual("<div attr1=\"test\">This is a test</div>", result);
        }
    }
}