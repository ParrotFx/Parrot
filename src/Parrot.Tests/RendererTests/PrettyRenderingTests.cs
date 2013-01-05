// -----------------------------------------------------------------------
// <copyright file="PrettyRenderingTests.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Parrot.Tests.RendererTests
{
    using NUnit.Framework;
    using Parrot.Infrastructure;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class PrettyRenderingTests : TestRenderingBase
    {
        private class PrettyRenderingHost : MemoryHost
        {
            public override IParrotWriter CreateWriter()
            {
                return new PrettyStringWriter();
            }
        }

        [Test]
        public void SingleIndentation()
        {
            Assert.AreEqual("<div>\r\n\tthis is a test\r\n</div>\r\n", Render("div > @\"this is a test\"", new PrettyRenderingHost()));
            Assert.AreEqual("<html>\r\n\t<div>\r\n\t\t1\r\n\t</div>\r\n</html>\r\n", Render("html > div > @\"1\"", new PrettyRenderingHost()));
            Assert.AreEqual("<html>\r\n\t<div>\r\n\t\t1\r\n\t</div>\r\n\t<div>\r\n\t\t1\r\n\t</div>\r\n</html>\r\n", Render("html { div { @\"1\" } div { @\"1\" } }", new PrettyRenderingHost()));
        }

        [Test]
        public void Tests()
        {
        }
    }
}