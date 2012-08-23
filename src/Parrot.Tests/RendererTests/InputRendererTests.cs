using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Parrot.Infrastructure;
using Parrot.Mvc;
using Parrot.Mvc.Renderers;
using Parrot.Nodes;

namespace Parrot.Tests
{
    using System.IO;

    [TestFixture]
    public class InputRendererTests : TestRenderingBase
    {
        private Document Parse(string text, IHost host)
        {
            Parser.Parser parsr = new Parser.Parser();
            Document document;

            parsr.Parse(new StringReader(text), host, out document);

            return document;
        }

        [Test]
        public void InputWithoutAnythingSpecialReturnsBasicInputElement()
        {
            string block = "input";
            var nodes = Parse(block, new MemoryHost());

            IRendererFactory factory = new RendererFactory();

            factory.RegisterFactory("input", new InputRenderer());
            var renderer = factory.GetRenderer(nodes.Children.First().Name);

            var result = renderer.Render(nodes.Children.First(), null);

            Assert.AreEqual("<input />", result);
        }

        [Test]
        public void InputAttributes()
        {
            Assert.AreEqual("<input checked=\"checked\" type=\"checkbox\" />", Render("input[type=\"checkbox\" checked]"));

            //input stuff needs special overrides for checked
            Assert.AreEqual("<input checked=\"checked\" type=\"checkbox\" />", Render("input[type=\"checkbox\" checked=true]"));
            Assert.AreEqual("<input type=\"checkbox\" />", Render("input[type=\"checkbox\" checked=false]"));
            Assert.AreEqual("<input type=\"checkbox\" />", Render("input[type=\"checkbox\" checked=null]"));
            Assert.AreEqual("<input checked=\"checked\" type=\"checkbox\" />", Render("input[checked type=\"checkbox\"]"));
        }

        [Test]
        public void InputWithAttributsReturnsElementWithAttributes()
        {
            string block = "input[attr=\"value\"]";

            var nodes = Parse(block, new MemoryHost());

            IRendererFactory factory = new RendererFactory();

            factory.RegisterFactory("input", new InputRenderer());
            var renderer = factory.GetRenderer(nodes.Children.First().Name);

            var result = renderer.Render(nodes.Children.First(), null);

            Assert.AreEqual("<input attr=\"value\" />", result);
        }

        [Test]
        public void InputWithTypeShortcuts()
        {
            Assert.AreEqual("<input type=\"submit\" />", Render("input:submit"));
            Assert.AreEqual("<input type=\"text\" />", Render("input:text"));
            Assert.AreEqual("<input type=\"checkbox\" />", Render("input:checkbox"));
            Assert.AreEqual("<input type=\"radio\" />", Render("input:radio"));
            Assert.AreEqual("<input type=\"password\" />", Render("input:password"));
            Assert.AreEqual("<input type=\"reset\" />", Render("input:reset"));
            Assert.AreEqual("<input type=\"file\" />", Render("input:file"));
            Assert.AreEqual("<input type=\"image\" />", Render("input:image"));
        }
    }
}
