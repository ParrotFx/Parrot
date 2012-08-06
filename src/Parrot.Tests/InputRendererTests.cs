using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Parrot.Mvc;
using Parrot.Mvc.Renderers;
using Parrot.Nodes;

namespace Parrot.Tests
{
    using System.IO;

    [TestFixture]
    public class InputRendererTests
    {
        private Document Parse(string text)
        {
            Parser.Parser parsr = new Parser.Parser();
            Document document;

            parsr.Parse(new StringReader(text), out document);

            return document;
        }

        [Test]
        public void InputWithoutAnythingSpecialReturnsBasicInputElement()
        {
            string block = "input";
            var nodes = Parse(block);

            IRendererFactory factory = new RendererFactory();

            factory.RegisterFactory("input", new InputRenderer());
            var renderer = factory.GetRenderer(nodes.Children.First().Name);

            var result = renderer.Render(nodes.Children.First(), null);

            Assert.AreEqual("<input />", result);
        }

        [Test]
        public void InputWithAttributsReturnsElementWithAttributes()
        {
            string block = "input[attr=\"value\"]";

            var nodes = Parse(block);

            IRendererFactory factory = new RendererFactory();

            factory.RegisterFactory("input", new InputRenderer());
            var renderer = factory.GetRenderer(nodes.Children.First().Name);

            var result = renderer.Render(nodes.Children.First(), null);

            Assert.AreEqual("<input attr=\"value\" />", result);
        }
    }
}
