namespace Parrot.Tests.Parser
{
    using System;
    using NUnit.Framework;
    using Parrot.Nodes;
    using Parrot.Parser.ErrorTypes;

    [TestFixture]
    public class AttributeTests : ParrotParserTestsBase
    {
        [Test]
        public void ElementWithSingleAttributeProducesBlockElementWithAttributes()
        {
            var document = Parse("div[attr1='value1']");
            Assert.AreEqual(1, document.Children[0].Attributes.Count);
        }

        [Test]
        public void ElementWithMultipleAttributesProducesBlockElementWithMultipleAttributes()
        {
            var document = Parse("div[attr1='value1' attr2='value2']");
            Assert.AreEqual(2, document.Children[0].Attributes.Count);
        }

        [Test]
        public void ElementWithAttributeValueNotSurroundedByQuotesProducesAttributeWithValueTypeAsProperty()
        {
            var document = Parse("div[attr1=Value]");
            Assert.AreEqual(1, document.Children[0].Attributes.Count);
            //Assert.AreEqual(ValueType.Property, document.Children[0].Attributes[0].ValueType);
        }

        [Test]
        public void ElementWithAttributeValueSetTothisProducesAttributeWithValueTypeAsLocal()
        {
            var document = Parse("div[attr1=this]");
            Assert.AreEqual(1, document.Children[0].Attributes.Count);
            //Assert.AreEqual(ValueType.Local, document.Children[0].Attributes[0].ValueType);
        }

        [Test]
        public void ElementWithAttributeValueAndChildPrintsOutProperAttributes()
        {
            var document = Parse("a[href='/blah/blah'] |Child\r\n");
            Assert.AreEqual("href", document.Children[0].Attributes[0].Key);
        }

        [Test]
        public void ElementWithAttributeWithNoValueProducesAttributeWithValueSetToNull()
        {
            var document = Parse("div[attr]");
            Assert.IsNull(document.Children[0].Attributes[0].Value);
            Assert.AreEqual("attr", document.Children[0].Attributes[0].Key);
        }

        [Test]
        public void ElementWithOutElementDeclarationButWithClassDeclarationCreatesDivElement()
        {
            var document = Parse(".sample-class");
            Assert.IsNullOrEmpty(null, document.Children[0].Name);
            Assert.AreEqual("class", document.Children[0].Attributes[0].Key);
            Assert.IsInstanceOf<StringLiteral>(document.Children[0].Attributes[0].Value);
            Assert.AreEqual("sample-class", (document.Children[0].Attributes[0].Value as StringLiteral).Values[0].Data);
            //Assert.AreEqual(ValueType.StringLiteral, document.Children[0].Attributes[0].ValueType);
        }

        [Test]
        public void ElementWithMissingAttributeValueButWithEqualsAddsErrorToDocumentErrors()
        {
            var document = Parse("div[attr1=]");
            Assert.IsAssignableFrom<AttributeValueMissing>(document.Errors[0]);
        }

        [Test]
        public void ElementWithEmptyAttributeBracketsAddsErrorToDocumentErrors()
        {
            var document = Parse("div[]");
            Assert.IsAssignableFrom<AttributeListEmpty>(document.Errors[0]);
        }

        [Test]
        public void ElementWithAttributeValueOnlyAddsErrorToDocumentErrors()
        {
            var document = Parse("div[=\"value only\"]");
            Assert.IsAssignableFrom<AttributeIdentifierMissing>(document.Errors[0]);
        }

        [Test]
        public void AttributeValueWithMissingClosingQuoteAddsErrorToDocumentErrors()
        {
            var document = Parse("div[attr1=\"missing closing quote");
            Assert.IsAssignableFrom<EndOfStreamException>(document.Errors[0]);
        }

        [Test]
        public void StatementWithGTChildCreatesBlockWithOneChild()
        {
            var document = Parse("div > span");
            Assert.AreEqual("div", document.Children[0].Name);
            Assert.AreEqual("span", document.Children[0].Children[0].Name);

            document = Parse("div > span > span");
            Assert.AreEqual("div", document.Children[0].Name);
            Assert.AreEqual("span", document.Children[0].Children[0].Name);
            Assert.AreEqual("span", document.Children[0].Children[0].Children[0].Name);
        }

        [TestCase("@", StringLiteralPartType.Encoded)]
        [TestCase("=", StringLiteralPartType.Raw)]
        public void StringLiteralParserTests(string delimiter, StringLiteralPartType encoding)
        {
            var parts = new StringLiteral(String.Format("\"this {0}is awesome {0}right\"", delimiter)).Values;

            Assert.AreEqual(4, parts.Count);
            Assert.AreEqual(StringLiteralPartType.Literal, parts[0].Type);
            Assert.AreEqual(encoding, parts[1].Type);
            Assert.AreEqual(StringLiteralPartType.Literal, parts[2].Type);
            Assert.AreEqual(encoding, parts[1].Type);

            Assert.AreEqual("this ", parts[0].Data);
            Assert.AreEqual("is", parts[1].Data);
            Assert.AreEqual(" awesome ", parts[2].Data);
            Assert.AreEqual("right", parts[3].Data);

            parts = new StringLiteral(String.Format("\"this contains a {0} but not a keyword\"", delimiter)).Values;
            Assert.AreEqual(1, parts.Count);
            Assert.AreEqual(StringLiteralPartType.Literal, parts[0].Type);

            parts = new StringLiteral(String.Format("\"{0}keyword_only\"", delimiter)).Values;
            Assert.AreEqual(1, parts.Count);
            Assert.AreEqual(encoding, parts[0].Type);
            Assert.AreEqual("keyword_only", parts[0].Data);

            parts = new StringLiteral(String.Format("\"{0}keyword_first followed by more words\"", delimiter)).Values;
            Assert.AreEqual(2, parts.Count);
            Assert.AreEqual(encoding, parts[0].Type);
            Assert.AreEqual(StringLiteralPartType.Literal, parts[1].Type);

            parts = new StringLiteral(String.Format("\"{0}keyword.with.dot\"", delimiter)).Values;
            Assert.AreEqual(1, parts.Count);
            Assert.AreEqual(encoding, parts[0].Type);
            Assert.AreEqual("keyword.with.dot", parts[0].Data);

            parts = new StringLiteral(String.Format("\"this is an {0}{0} escaped colon\"", delimiter)).Values;
            Assert.AreEqual(1, parts.Count);
            Assert.AreEqual(String.Format("this is an {0} escaped colon", delimiter), parts[0].Data);

            parts = new StringLiteral(String.Format("\"{0}keyword_only_endsin. a dot\"", delimiter)).Values;
            Assert.AreEqual(2, parts.Count);
            Assert.AreEqual(encoding, parts[0].Type);
            Assert.AreEqual(StringLiteralPartType.Literal, parts[1].Type);
            Assert.AreEqual("keyword_only_endsin", parts[0].Data);
            Assert.AreEqual(". a dot", parts[1].Data);
        }
    }
}