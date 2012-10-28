// -----------------------------------------------------------------------
// <copyright file="PrettyRenderingTests.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System.IO;
using NUnit.Framework;
using Parrot.Infrastructure;

namespace Parrot.Tests.RendererTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class PrettyRenderingTests : TestRenderingBase
    {

        private Host GetHost()
        {
            var host = new MemoryHost();
            host.DependencyResolver.Register(typeof(IParrotWriter), () => new PrettyStringWriter());

            return host;
        }

        [Test]
        public void SingleIndentation()
        {
            Assert.AreEqual("<div>\r\n\tthis is a test\r\n</div>\r\n", Render("div > :\"this is a test\"", GetHost()));
            Assert.AreEqual("<html>\r\n\t<div>\r\n\t\t1\r\n\t</div>\r\n</html>\r\n", Render("html > div > :\"1\"", GetHost()));
            Assert.AreEqual("<html>\r\n\t<div>\r\n\t\t1\r\n\t</div>\r\n\t<div>\r\n\t\t1\r\n\t</div>\r\n</html>\r\n", Render("html { div { :\"1\" } div { :\"1\" } }", GetHost()));
        }

        [Test]
        public void Tests()
        {
            
        }

    }
}
