namespace Parrot.Tests.Parser
{
    using System;
    using NUnit.Framework;
    using Parrot.Nodes;
    using Parrot.Parser.ErrorTypes;

    [TestFixture]
    public class IdTests : ParrotParserTestsBase
    {
        [TestCase("div", "sample-id")]
        [TestCase("", "sample-id")]
        public void ElementWithIdProducesBlockElementWithIdAttribute(string element, string id)
        {
            var document = Parse(String.Format("{0}#{1}", element, id));
            Assert.AreEqual(element, document.Children[0].Name);
            Assert.AreEqual("id", document.Children[0].Attributes[0].Key);
            Assert.IsInstanceOf<StringLiteral>(document.Children[0].Attributes[0].Value);
            Assert.AreEqual(id, (document.Children[0].Attributes[0].Value as StringLiteral).Values[0].Data);
        }

        [Test]
        public void ElementWithTwoOrMoreIds()
        {
            var document = Parse("div#first-id#second-id#third-id");
            var error1 = document.Errors[0] as MultipleIdDeclarations;
            var error2 = document.Errors[1] as MultipleIdDeclarations;
            Assert.AreEqual("second-id", error1.Id);
            Assert.AreEqual("third-id", error2.Id);
        }

        [Test]
        public void ElementWithMultipleIdsThrowsParserException()
        {
            var document = Parse("div#first-id#second-id");
            var error = document.Errors[0] as MultipleIdDeclarations;
            Assert.AreEqual("second-id", error.Id);
        }

        [Test]
        public void ElementWithEmptyIdDeclaration()
        {
            var document = Parse("div#");
            Assert.IsAssignableFrom<MissingIdDeclaration>(document.Errors[0]);
        }
    }
}