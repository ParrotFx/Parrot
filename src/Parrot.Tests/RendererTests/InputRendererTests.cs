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
    using Renderers;
    using Renderers.Infrastructure;

    [TestFixture]
    public class InputRendererTests : TestRenderingBase
    {
        private Document Parse(string text, IHost host)
        {
            Parser.Parser parser = new Parser.Parser(host);
            Document document;

            parser.Parse(text, out document);

            return document;
        }

        [Test]
        public void InputWithoutAnythingSpecialReturnsBasicInputElement()
        {
            var host = new AspNetHost();

            string block = "input";
            var nodes = Parse(block, host);

            Assert.AreEqual("<input />", Render(block, host));

            //var result = renderer.Render(nodes.Children.First(), null);

            //Assert.AreEqual("<input />", result);
        }

        [Test]
        public void InputAttributes()
        {
            //Assert.AreEqual("<input checked=\"checked\" type=\"checkbox\" />", Render("input[type=\"checkbox\" checked]", new AspNetHost()));

            //input stuff needs special overrides for checked
            Assert.AreEqual("<input checked=\"checked\" type=\"checkbox\" />", Render("input[type=\"checkbox\" checked=true]", new AspNetHost()));
            Assert.AreEqual("<input type=\"checkbox\" />", Render("input[type=\"checkbox\" checked=false]", new AspNetHost()));
            Assert.AreEqual("<input type=\"checkbox\" />", Render("input[type=\"checkbox\" checked=null]", new AspNetHost()));

            Assert.AreEqual("<input checked=\"checked\" type=\"radio\" />", Render("input[type=\"radio\" checked=true]", new AspNetHost()));
            Assert.AreEqual("<input type=\"radio\" />", Render("input[type=\"radio\" checked=false]", new AspNetHost()));
            Assert.AreEqual("<input type=\"radio\" />", Render("input[type=\"radio\" checked=null]", new AspNetHost()));
        }

        [Test]
        public void InputWithAttributsReturnsElementWithAttributes()
        {
            var host = new MemoryHost();
            string block = "input[attr=\"value\"]";

            Assert.AreEqual("<input attr=\"value\" />", Render(block, host));
            //var result = renderer.Render(nodes.Children.First(), null);

            //Assert.AreEqual("<input attr=\"value\" />", result);
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
            Assert.AreEqual("<input type=\"hidden\" />", Render("input:hidden"));
        }
    }
}
