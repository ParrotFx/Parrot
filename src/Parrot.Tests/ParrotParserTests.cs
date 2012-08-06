// -----------------------------------------------------------------------
// <copyright file="ParrotParserTests.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using Parrot.Mvc.Renderers;

namespace Parrot.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using Nodes;
    using Parrot;
    using NUnit.Framework;
    using Parser;
    using ValueType = Nodes.ValueType;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [TestFixture]
    public class ParrotParserTests
    {
        //so what are we tsting
        //block name == blcok name
        //block followed by block
        //block followed by ; with block
        //attribute name/values
        //parameter name/values
        public static Document Parse(string text)
        {
            Parser parsr = new Parser();
            Document document;

            parsr.Parse(new StringReader(text), out document);

            return document;
        }

        [TestCase("div")]
        [TestCase("a")]
        [TestCase("span")]
        public void ElementProducesBlockElement(string element)
        {
            var document = Parse(element);
            Assert.IsNotNull(document);
            Assert.AreEqual(element, document.Children[0].Name);
        }

        [TestCase("div1", "div2")]
        public void ElementFollowedByWhitespaceAndAnotherElementProduceTwoBlockElements(string element1, string element2)
        {
            var document = Parse(string.Format("{0} {1}", element1, element2));
            Assert.AreEqual(2, document.Children.Count);
        }


        public class IdTests
        {
            [TestCase("div", "sample-id")]
            public void ElementWithIdProducesBlockElementWithIdAttribute(string element, string id)
            {
                var document = Parse(string.Format("{0}#{1}", element, id));
                Assert.AreEqual(element, document.Children[0].Name);
                Assert.AreEqual("id", document.Children[0].Attributes[0].Key);
                Assert.AreEqual(id, document.Children[0].Attributes[0].Value);
            }

            [Test]
            public void ElementWithMultipleIdsThrowsParserException()
            {
                Assert.Throws<ParserException>(() =>
                {
                    var document = Parse("div#first-id#second-id");
                });
            }
        }

        public class ClassTests
        {
            [TestCase("div", "sample-class")]
            public void ElementWithIdProducesBlockElementWithClassAttribute(string element, string @class)
            {
                var document = Parse(string.Format("{0}.{1}", element, @class));
                Assert.AreEqual("class", document.Children[0].Attributes[0].Key);
                Assert.AreEqual(@class, document.Children[0].Attributes[0].Value);
            }

            [TestCase("div", "class1", "class2", "class3")]
            public void ElementWithMultipleClassProducesBlockElementWithClassElementAndSpaceSeparatedClasses(string element, params string[] classes)
            {
                var document = Parse(string.Format("{0}.{1}", element, string.Join(".", classes)));
                Assert.AreEqual("class", document.Children[0].Attributes[0].Key);
                for (int i = 0; i < classes.Length; i++)
                {
                    Assert.AreEqual(classes[i], document.Children[0].Attributes[i].Value);
                }
            }
        }

        public class AttributeTests
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
                Assert.AreEqual(ValueType.Property, document.Children[0].Attributes[0].ValueType);
            }

            [Test]
            public void ElementWithAttributeValueSetTothisProducesAttributeWithValueTypeAsLocal()
            {
                var document = Parse("div[attr1=this]");
                Assert.AreEqual(1, document.Children[0].Attributes.Count);
                Assert.AreEqual(ValueType.Local, document.Children[0].Attributes[0].ValueType);
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
                Assert.AreEqual("sample-class", document.Children[0].Attributes[0].Value);
            }

            [Test]
            public void ElementWithInvalidAttributeDeclarationsThrowsParserException()
            {
                Assert.Throws<ParserException>(() => Parse("div[attr1=]"));
                Assert.Throws<ParserException>(() => Parse("div[]"));
                Assert.Throws<ParserException>(() => Parse("div[=\"value only\"]"));
                Assert.Throws<ParserException>(() => Parse("div[attr1=\"missing closing quote]"));
                Assert.Throws<ParserException>(() => Parse("div[attr1='missing closing quote]")); //why is this one failing
            }
        }
    }

}
