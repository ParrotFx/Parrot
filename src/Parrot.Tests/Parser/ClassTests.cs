namespace Parrot.Tests.Parser
{
    using System;
    using NUnit.Framework;
    using Parrot.Nodes;
    using Parrot.Parser.ErrorTypes;

    [TestFixture]
    public class ClassTests : ParrotParserTestsBase
    {
        [TestCase("div", "sample-class")]
        public void ElementWithIdProducesBlockElementWithClassAttribute(string element, string @class)
        {
            var document = Parse(String.Format("{0}.{1}", element, @class));
            Assert.AreEqual("class", document.Children[0].Attributes[0].Key);
            Assert.IsInstanceOf<StringLiteral>(document.Children[0].Attributes[0].Value);
            Assert.AreEqual(@class, (document.Children[0].Attributes[0].Value as StringLiteral).Values[0].Data);
        }

        [TestCase("div", "class1", "class2", "class3")]
        public void ElementWithMultipleClassProducesBlockElementWithClassElementAndSpaceSeparatedClasses(string element, params string[] classes)
        {
            var document = Parse(String.Format("{0}.{1}", element, String.Join(".", classes)));
            Assert.AreEqual("class", document.Children[0].Attributes[0].Key);
            for (int i = 0; i < classes.Length; i++)
            {
                Assert.IsInstanceOf<StringLiteral>(document.Children[0].Attributes[i].Value);
                Assert.AreEqual(classes[i], (document.Children[0].Attributes[i].Value as StringLiteral).Values[0].Data);
            }
        }

        [Test]
        public void ElementWithEmptyClassDeclaration()
        {
            var document = Parse("div.");
            Assert.IsAssignableFrom<MissingClassDeclaration>(document.Errors[0]);
        }
    }
}