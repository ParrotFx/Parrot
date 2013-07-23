namespace Parrot.Tests.Parser
{
    using NUnit.Framework;

    [TestFixture]
    public class SiblingTests : ParrotParserTestsBase
    {
        [Test]
        public void RandomTestUntilIComeUpWithAName()
        {
            var document = Parse("h2 > \"Render\" @sibling");
            Assert.AreEqual(2, document.Children.Count);
            Assert.AreEqual("h2", document.Children[0].Name);
            Assert.AreEqual("string", document.Children[1].Name);
        }
    }
}