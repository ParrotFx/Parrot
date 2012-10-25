// -----------------------------------------------------------------------
// <copyright file="RenderTests.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System.Dynamic;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using Parrot.Mvc;
using Parrot.Mvc.Renderers;
using Parrot.Renderers;
using Parrot.Renderers.Infrastructure;
using HtmlRenderer = Parrot.Renderers.HtmlRenderer;

namespace Parrot.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [TestFixture]
    public class RenderTests : TestRenderingBase
    {
        //https://github.com/visionmedia/jade/blob/master/test/jade.test.js

        [Test]
        public void ThisKeywordHandling()
        {
            var text = "div(Model) > :this";
            Assert.AreEqual("<div>testing this</div>", Render(text, new { Model = "testing this" }));
        }

        [Test]
        public void DocumentHostWithAlternateProeprtyName()
        {
            var text = "div(Request.IsAuthenticated) > :this";

            Assert.AreEqual("<div>True</div>", Render(text, new Dictionary<string, object> { {"Request", new { IsAuthenticated = true }}}));
        }

        [Test]
        public void ViewBagTest()
        {
            var text = "div(ViewBag.Value) > :this";
            dynamic viewBag = new ExpandoObject();
            viewBag.Value = true;

            Assert.AreEqual("<div>True</div>", Render(text, new Dictionary<string, object> { { "ViewBag", viewBag }}));
        }

        [Test]
        public void ForeachRendererTests()
        {
            object model = new[] { 1, 2 };
            Assert.AreEqual("<div>1</div><div>2</div>", Render("foreach(Model) { div > :this }", model));
            Assert.AreEqual("<div>1</div><div>2</div>", Render("foreach(Model) { div > :this }", model));

            Assert.Throws<InvalidCastException>(() => Render("foreach(Model) { div(this) }", new { Model = new { Item = 1 } }));
        }

        [Test]
        public void StandardSingleFileSimpleRenderingLayout()
        {
            var host = new MemoryHost();

            //create a view engine and register it
            var viewEngine = new ParrotViewEngine(host);
            host.DependencyResolver.Register(typeof(IViewEngine), () => viewEngine);

            var testFile = "div > \"testing\"";

            var pathResolver = new Mock<IPathResolver>();
            pathResolver.Setup(p => p.OpenFile("index.parrot")).Returns(new MemoryStream(Encoding.Default.GetBytes(testFile)));

            //register the path resolver with the dependency resolver
            host.DependencyResolver.Register(typeof(IPathResolver), () => pathResolver.Object);

            //default renderer
            var rendererFactory = new RendererFactory(host);
            rendererFactory.RegisterFactory("*", new HtmlRenderer(host, rendererFactory));
            rendererFactory.RegisterFactory("string", new StringLiteralRenderer(host, rendererFactory));
            rendererFactory.RegisterFactory("doctype", new DocTypeRenderer(host));
            rendererFactory.RegisterFactory("foreach", new ForeachRenderer(host, rendererFactory));
            rendererFactory.RegisterFactory(new[] { "ul", "ol" }, new ListRenderer(host, rendererFactory));
            rendererFactory.RegisterFactory(
                new[] { "base", "basefont", "frame", "link", "meta", "area", "br", "col", "hr", "img", "param" },
                new SelfClosingRenderer(host, rendererFactory)
            );

            host.DependencyResolver.Register(typeof(IRendererFactory), () => rendererFactory);

            ////TODO: Figure this out later...
            var view = new ParrotView(host, "index.parrot");
            StringBuilder sb = new StringBuilder();
            view.Render(null, new StringWriter(sb));
            Assert.AreEqual("<div>testing</div>", sb.ToString());
        }

        [Test]
        public void LayoutRendererTests()
        {
            var host = new MemoryHost();

            //create a view engine
            var viewEngine = new ParrotViewEngine(host);

            host.DependencyResolver.Register(typeof(IViewEngine), () => viewEngine);

            var layout = "html > body > content";
            var testFile = "layout(\"layout\") { div > \"testing\" }";

            var pathResolver = new Mock<IPathResolver>();
            pathResolver.Setup(p => p.OpenFile("index.parrot")).Returns(new MemoryStream(Encoding.Default.GetBytes(testFile)));
            pathResolver.Setup(p => p.OpenFile("~/Views/Shared/layout.parrot")).Returns(new MemoryStream(Encoding.Default.GetBytes(layout)));
            pathResolver.Setup(p => p.FileExists(It.IsAny<string>())).Returns(false);
            pathResolver.Setup(p => p.FileExists("~/Views/Shared/layout.parrot")).Returns(true);
            pathResolver.Setup(p => p.VirtualFilePath("~/Views/Shared/layout.parrot")).Returns("~/Views/Shared/layout.parrot");

            host.DependencyResolver.Register(typeof(IPathResolver), () => pathResolver.Object);

            var rendererFactory = new RendererFactory(host);
            rendererFactory.RegisterFactory("*", new HtmlRenderer(host, rendererFactory));
            rendererFactory.RegisterFactory("string", new StringLiteralRenderer(host, rendererFactory));
            rendererFactory.RegisterFactory("doctype", new DocTypeRenderer(host));
            rendererFactory.RegisterFactory("foreach", new ForeachRenderer(host, rendererFactory));
            rendererFactory.RegisterFactory("layout", new LayoutRenderer(host, rendererFactory));
            rendererFactory.RegisterFactory("content", new ContentRenderer(host, rendererFactory));
            rendererFactory.RegisterFactory(new[] { "ul", "ol" }, new ListRenderer(host, rendererFactory));
            rendererFactory.RegisterFactory(
                new[] { "base", "basefont", "frame", "link", "meta", "area", "br", "col", "hr", "img", "param" },
                new SelfClosingRenderer(host, rendererFactory)
            );

            host.DependencyResolver.Register(typeof(IRendererFactory), () => rendererFactory);

            ////TODO: Figure this out later...
            var view = new ParrotView(host, "index.parrot");
            StringBuilder sb = new StringBuilder();
            view.Render(null, new StringWriter(sb));
            Assert.AreEqual("<html><body><div>testing</div></body></html>", sb.ToString());
        }

        [Test]
        public void DoctypeRendererTests()
        {
            Assert.AreEqual("<!DOCTYPE html>", Render("doctype"));
            Assert.AreEqual("<!DOCTYPE xhtml>", Render("doctype(\"xhtml\")"));
        }

        [Test]
        public void RenderATagWithModel()
        {
            var block = "a(Model)[href=:FirstName] > :FirstName";
            var result = Render(block, new { Model = new { FirstName = "Ben" } });
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

            Assert.AreEqual("<img src=\"/foo.png\" />", Render("img[src=\"/foo.png\"]"));
            Assert.AreEqual("<img src=\"/foo.png\" />", Render("img[src = \"/foo.png\"]"));
            Assert.AreEqual("<img src=\"/foo.png\" />", Render("img[src=\"/foo.png\"]"));
            Assert.AreEqual("<img src=\"/foo.png\" />", Render("img[src = \"/foo.png\"]"));

            Assert.AreEqual("<img alt=\"just some foo\" src=\"/foo.png\" />", Render("img[src=\"/foo.png\" alt=\"just some foo\"]"));
            Assert.AreEqual("<img alt=\"just some foo\" src=\"/foo.png\" />", Render("img[src = \"/foo.png\" alt = \"just some foo\"]"));

            Assert.AreEqual("<p class=\"foo,bar,baz\"></p>", Render("p[class=\"foo,bar,baz\"]"));
            Assert.AreEqual("<a href=\"http://google.com\" title=\"Some : weird = title\"></a>", Render("a[href= \"http://google.com\" title= \"Some : weird = title\"]"));
            Assert.AreEqual("<label for=\"name\"></label>", Render("label[for=\"name\"]"));
            Assert.AreEqual("<meta content=\"width=device-width\" name=\"viewport\" />", Render("meta[name=\"viewport\" content=\"width==device-width\"]"));
            Assert.AreEqual("<div style=\"color= white\"></div>", Render("div[style=\"color= white\"]"));
            Assert.AreEqual("<div style=\"color:white\"></div>", Render("div[style=\"color::white\"]"));
            Assert.AreEqual("<p class=\"foo\"></p>", Render("p[class=\"foo\"]"));
            Assert.AreEqual("<p class=\"foo\"></p>", Render("p[class=\"foo\"]"));
            Assert.AreEqual("<p class=\"baz bar foo\"></p>", Render("p.bar.baz[class=\"foo\"]"));

            Assert.AreEqual("<p data-lang=\"en\"></p>", Render("p[data-lang = \"en\"]"));
            Assert.AreEqual("<p data-dynamic=\"true\"></p>", Render("p[data-dynamic= \"true\"]"));
            Assert.AreEqual("<p class=\"name\" data-dynamic=\"true\"></p>", Render("p[class= \"name\" data-dynamic= \"true\"]"));
            Assert.AreEqual("<p data-dynamic=\"true\"></p>", Render("p[data-dynamic= \"true\"]"));
            Assert.AreEqual("<p class=\"name\" data-dynamic=\"true\"></p>", Render("p[class= \"name\" data-dynamic= \"true\"]"));

            Assert.AreEqual("<p class=\"name\" data-dynamic=\"true\" yay=\"yay\"></p>", Render("p[class= \"name\" data-dynamic= \"true\" yay]"));

            Assert.AreEqual("<a data-foo=\"{ foo: &#39;bar&#39;, bar= &#39;baz&#39; }\"></a>", Render("a[data-foo = \"{ foo: 'bar', bar= 'baz' }\"]"));

            Assert.AreEqual("<meta content=\"IE=edge,chrome=1\" http-equiv=\"X-UA-Compatible\" />", Render("meta[http-equiv=\"X-UA-Compatible\" content=\"IE==edge,chrome==1\"]"));

            Assert.AreEqual("<div style=\"background: url(/images/test.png)\"></div>", Render("div[style= \"background: url(/images/test.png)\"]"));
            Assert.AreEqual("<div style=\"background = url(/images/test.png)\"></div>", Render("div[style= \"background = url(/images/test.png)\"]"));

            Assert.AreEqual("<rss xmlns:atom=\"atom\"></rss>", Render("rss[xmlns:atom=\"atom\"]"));
            Assert.AreEqual("<p></p>", Render("p(Model)[id=name]", new { name = "" }));
            Assert.AreEqual("<p id=\"tj\"></p>", Render("p(Model)[id= name]", new { name = "tj" }));
            Assert.AreEqual("<p id=\"something\"></p>", Render("p(Model)[id='something']", new { name = "" }));
        }

        [Test]
        public void TagOverrideRendering()
        {
            Assert.AreEqual("<ul><li class=\"sample-class\"></li></ul>", Render("ul { .sample-class }"));
            Assert.AreEqual("<div class=\"sample-class\"></div>", Render(".sample-class"));
        }

        [Test]
        public void StringLiteralPipeTests()
        {
            Assert.AreEqual("<div>this is a string literal test</div>", Render("div { |this is a string literal test\r\n}"));
            Assert.AreEqual("<div>12</div>", Render("div { |1\r\n|2\r\n}"));
            Assert.AreEqual("this is a string literal test", Render("|this is a string literal test\r"));
        }

        [Test]
        public void CssIdentifier()
        {
            Assert.AreEqual("<div class=\"sample-class\"></div>", Render(".sample-class"));
        }
    }
}
