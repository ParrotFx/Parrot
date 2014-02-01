namespace Parrot.Tests.Parser
{
    using NUnit.Framework;

    public class ClimbUpTests : ParrotParserTestsBase
    {
        [Test]
        public void ElementInChildElementWithCaretClimbsUp()
        {
            var document = Parse("div>p^p");
            Assert.AreEqual(0, document.Errors.Count);
            Assert.AreEqual("div", document.Children[0].Name);
            Assert.AreEqual("p", document.Children[0].Children[0].Name);
            Assert.AreEqual("p", document.Children[0].Children[1].Name);
        }

        [Test]
        public void ElementInChildElementWithCaretClimbsUpTwoLevels()
        {
            var document = Parse("root>a>b^^a2");

            Assert.AreEqual(0, document.Errors.Count);
            Assert.AreEqual("root", document.Children[0].Name);
            Assert.AreEqual("a", document.Children[0].Children[0].Name);
            Assert.AreEqual("b", document.Children[0].Children[0].Children[0].Name);
            Assert.AreEqual("a2", document.Children[0].Children[1].Name);
        }

        [Test]
        public void ElementInChildElementWithMultipleCaretsClimbsUpAboveRootShowsError()
        {
            var document = Parse("div^p");
            Assert.AreEqual(1, document.Errors.Count);
        }

        [Test]
        public void ElementInChildElementWithMultipleCaretsClimbsUpAboveRootShowsErrorFromChild()
        {
            var document = Parse("div>p^^p");
            Assert.AreEqual(1, document.Errors.Count);
        }
    }
}