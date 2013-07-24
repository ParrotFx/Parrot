namespace Parrot.Tests.Parser
{
    using NUnit.Framework;

    [TestFixture]
    public class ColonTests : ParrotParserTestsBase
    {
        [Test]
        public void ColonWithChildren()
        {
            var document = Parse("pre { @Item3 }");
        }

        [Test]
        public void EqualWithChild()
        {
            var document = Parse("pre { =Item3 }");
        }

        [Test]
        public void EndOfLine()
        {
            var document = Parse("pre @Item3");
            Assert.AreEqual(2, document.Children.Count);
            Assert.AreEqual(0, document.Errors.Count);
        }
    }
}