// -----------------------------------------------------------------------
// <copyright file="RenderTests.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using Moq;
using NUnit.Framework;
using Parrot.Infrastructure;
using Parrot.Mvc;
using Parrot.Mvc.Renderers;
using Parrot.Nodes;

namespace Parrot.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [TestFixture]
    public class RenderTests
    {
        //https://github.com/visionmedia/jade/blob/master/test/jade.test.js
        public IRendererFactory GetFactory()
        {
            RendererFactory factory = new RendererFactory();

            factory.RegisterFactory(new[] { "base", "basefont", "frame", "link", "meta", "area", "br", "col", "hr", "img", "param" }, new SelfClosingRenderer());
            factory.RegisterFactory("doctype", new DocTypeRenderer());
            factory.RegisterFactory("rawoutput", new RawOutputRenderer());
            factory.RegisterFactory("output", new OutputRenderer());
            factory.RegisterFactory("input", new InputRenderer());
            factory.RegisterFactory("string", new StringLiteralRenderer());
            factory.RegisterFactory("foreach", new ForeachRenderer());

            //default renderer
            factory.RegisterFactory("*", new HtmlRenderer());

            return factory;
        }

        private string Render(string parrot, object model)
        {
            var resolver = new Mock<IDependencyResolver>();
            resolver.Setup(f => f.Get<IRendererFactory>()).Returns(GetFactory());

            Host.DependencyResolver = resolver.Object;

            Parser.Parser parser = new Parser.Parser();
            Document document;

            parser.Parse(parrot, out document);
            var factory = GetFactory();

            StringBuilder sb = new StringBuilder();
            foreach (var element in document.Children)
            {
                var renderer = factory.GetRenderer(element.BlockName);
                sb.AppendLine(renderer.Render(element, model));
            }

            return sb.ToString().Trim();
        }

        private string Render(string parrot)
        {
            return Render(parrot, null);
        }

        
        [Test]
        public void ForeachRendererTests()
        {
            object model = new[] {1, 2};
            Assert.AreEqual("<div>1</div><div>2</div>", Render("foreach { div(this) }", model));
        }

        [Test]
        public void RenderATagWithModel()
        {
            var block = "a[href=FirstName](FirstName)";
            var result = Render(block, new { FirstName = "Ben" });
            Assert.AreEqual("<a href=\"Ben\">Ben</a>", result);
        }

        [Test]
        public void TestAttributes()
        {
            Assert.AreEqual("<img src=\"&lt;script&gt;\" />", Render("img[src='<script>']"));

            Assert.AreEqual("<a data-attr=\"bar\"></a>", Render("a[data-attr='bar']"));
            Assert.AreEqual("<a data-attr=\"bar\" data-attr-2=\"baz\"></a>", Render("a[data-attr='bar' data-attr-2='baz']"));

            Assert.AreEqual("<a title=\"foo,bar\"></a>", Render("a[title= \"foo,bar\"]"));
            Assert.AreEqual("<a href=\"#\" title=\"foo,bar\"></a>", Render("a[title=\"foo,bar\" href=\"#\"]"));

            Assert.AreEqual("<p class=\"foo\"></p>", Render("p[class='foo']"));
            Assert.AreEqual("<input checked=\"checked\" type=\"checkbox\" />", Render("input[type=\"checkbox\" checked]"));

            //input stuff needs special overrides for checked

            Assert.AreEqual("<input checked=\"checked\" type=\"checkbox\" />", Render("input[type=\"checkbox\" checked=true]"));
            //Assert.AreEqual("<input type=\"checkbox\" />", Render("input[type=\"checkbox\" checked=false]"));
            //Assert.AreEqual("<input type=\"checkbox\" />", Render("input[type=\"checkbox\" checked=null]"));

            Assert.AreEqual("<img src=\"/foo.png\" />", Render("img[src=\"/foo.png\"]"));
            Assert.AreEqual("<img src=\"/foo.png\" />", Render("img[src = \"/foo.png\"]"));
            Assert.AreEqual("<img src=\"/foo.png\" />", Render("img[src=\"/foo.png\"]"));
            Assert.AreEqual("<img src=\"/foo.png\" />", Render("img[src = \"/foo.png\"]"));

            Assert.AreEqual("<img alt=\"just some foo\" src=\"/foo.png\" />", Render("img[src=\"/foo.png\" alt=\"just some foo\"]"));
            Assert.AreEqual("<img alt=\"just some foo\" src=\"/foo.png\" />", Render("img[src = \"/foo.png\" alt = \"just some foo\"]"));

            Assert.AreEqual("<p class=\"foo,bar,baz\"></p>", Render("p[class=\"foo,bar,baz\"]"));
            Assert.AreEqual("<a href=\"http://google.com\" title=\"Some : weird = title\"></a>", Render("a[href= \"http://google.com\" title= \"Some : weird = title\"]"));
            Assert.AreEqual("<label for=\"name\"></label>", Render("label[for=\"name\"]"));
            Assert.AreEqual("<meta content=\"width=device-width\" name=\"viewport\" />", Render("meta[name=\"viewport\" content=\"width=device-width\"]"));
            Assert.AreEqual("<div style=\"color= white\"></div>", Render("div[style=\"color= white\"]"));
            Assert.AreEqual("<div style=\"color:white\"></div>", Render("div[style=\"color:white\"]"));
            Assert.AreEqual("<p class=\"foo\"></p>", Render("p[class=\"foo\"]"));
            Assert.AreEqual("<p class=\"foo\"></p>", Render("p[class= \"foo\"]"));

            Assert.AreEqual("<p data-lang=\"en\"></p>", Render("p[data-lang = \"en\"]"));
            Assert.AreEqual("<p data-dynamic=\"true\"></p>", Render("p[data-dynamic= \"true\"]"));
            Assert.AreEqual("<p class=\"name\" data-dynamic=\"true\"></p>", Render("p[class= \"name\" data-dynamic= \"true\"]"));
            Assert.AreEqual("<p data-dynamic=\"true\"></p>", Render("p[data-dynamic= \"true\"]"));
            Assert.AreEqual("<p class=\"name\" data-dynamic=\"true\"></p>", Render("p[class= \"name\" data-dynamic= \"true\"]"));
            Assert.AreEqual("<p class=\"name\" data-dynamic=\"true\" yay=\"yay\"></p>", Render("p[class= \"name\" data-dynamic= \"true\" yay]"));

            Assert.AreEqual("<input checked=\"checked\" type=\"checkbox\" />", Render("input[checked type=\"checkbox\"]"));

            Assert.AreEqual("<a data-foo=\"{ foo: &#39;bar&#39;, bar= &#39;baz&#39; }\"></a>", Render("a[data-foo = \"{ foo: 'bar', bar= 'baz' }\"]"));

            Assert.AreEqual("<meta content=\"IE=edge,chrome=1\" http-equiv=\"X-UA-Compatible\" />", Render("meta[http-equiv=\"X-UA-Compatible\" content=\"IE=edge,chrome=1\"]"));

            Assert.AreEqual("<div style=\"background: url(/images/test.png)\"></div>", Render("div[style= \"background: url(/images/test.png)\"]"));
            Assert.AreEqual("<div style=\"background = url(/images/test.png)\"></div>", Render("div[style= \"background = url(/images/test.png)\"]"));

            Assert.AreEqual("<rss xmlns:atom=\"atom\"></rss>", Render("rss[xmlns:atom=\"atom\"]"));
            //Assert.AreEqual("<p></p>", Render("p[id=name]", new { name = "" }));
            //Assert.AreEqual("<p id=\"tj\"></p>", Render("p[id= name]", new { name = "tj" }));
            Assert.AreEqual("<p id=\"something\"></p>", Render("p[id='something']", new { name = "" }));
        }
    }
}
