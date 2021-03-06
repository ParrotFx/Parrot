﻿// -----------------------------------------------------------------------
// <copyright file="ParrotParserTests.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Parrot.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;
    using Parrot.Nodes;
    using Parrot.Parser;
    using Parrot.Parser.ErrorTypes;

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
            Parser parser = new Parser();
            Document document;

            parser.Parse(text, out document);

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
        [TestCase("div1", "div2", "div3")]
        [TestCase("div1", "div2", "div3", "div4")]
        public void ElementFollowedByWhitespaceAndAnotherElementProduceTwoBlockElements(params object[] elements)
        {
            var document = Parse(string.Join(" ", elements));
            Assert.AreEqual(elements.Length, document.Children.Count);
        }

        [TestCase("div1", "div2")]
        [TestCase("div1", "div2", "div3")]
        [TestCase("div1", "div2", "div3", "div4 |child\r\n")]
        public void ElementWithMultipleChildrenElements(params object[] elements)
        {
            var document = Parse("div { " + string.Join(" ", elements) + " }");
            Assert.AreEqual(elements.Length, document.Children[0].Children.Count);
        }

        [Test]
        public void StatementWithOneSibling()
        {
            var document = Parse("div1 + div2");
            Assert.AreEqual(2, document.Children.Count);
            Assert.AreEqual("div1", document.Children[0].Name);
            Assert.AreEqual("div2", document.Children[1].Name);
        }

        [Test]
        public void StatementWithChildFollowedByStatement()
        {
            var document = Parse("parent > child statement");
            Assert.AreEqual(2, document.Children.Count);
            Assert.AreEqual("parent", document.Children[0].Name);
            Assert.AreEqual("statement", document.Children[1].Name);
            Assert.AreEqual("child", document.Children[0].Children[0].Name);
        }

        [Test]
        public void StatementWithChildFollowedByStatementWithChild()
        {
            var document = Parse("parent > child statement > child2");
            Assert.AreEqual(2, document.Children.Count);
            Assert.AreEqual("parent", document.Children[0].Name);
            Assert.AreEqual("statement", document.Children[1].Name);
            Assert.AreEqual("child", document.Children[0].Children[0].Name);
        }

        [Test]
        public void StatementWithLiteralChildFollowedByStatementWithChild()
        {
            var document = Parse("parent |child\r\nstatement > child2");
            Assert.AreEqual(2, document.Children.Count);
            Assert.AreEqual("parent", document.Children[0].Name);
            Assert.AreEqual("statement", document.Children[1].Name);
            Assert.AreEqual("string", document.Children[0].Children[0].Name);
        }

        [Test]
        public void StatementWithLiteralChildFollowedByStatementWithLiteralChild()
        {
            //TODO: Remove \r\n
            //This appears to be a bug in the gold engine
            //when converted to new lexer/parser remove
            //the \r\n
            var document = Parse("parent |child\r\nstatement |child2\r\n");
            Assert.AreEqual(2, document.Children.Count);
            Assert.AreEqual("parent", document.Children[0].Name);
            Assert.AreEqual("statement", document.Children[1].Name);
            Assert.AreEqual("string", document.Children[0].Children[0].Name);
        }

        [Test]
        public void StatementWithTwoSiblings()
        {
            var document = Parse("div1 + div2 + div3");
            Assert.AreEqual(3, document.Children.Count);
            Assert.AreEqual("div1", document.Children[0].Name);
            Assert.AreEqual("div2", document.Children[1].Name);
            Assert.AreEqual("div3", document.Children[2].Name);
        }

        [Test]
        public void StatementWithChildrenIdentifiedAsSiblings()
        {
            var document = Parse("div1 > child1 + child2");
            Assert.AreEqual(1, document.Children.Count);
            var parent = document.Children[0];
            Assert.AreEqual(2, parent.Children.Count);
            Assert.AreEqual("child1", parent.Children[0].Name);
            Assert.AreEqual("child2", parent.Children[1].Name);
        }

        [Test]
        public void StatementWithChildrenIdentifiedAsSiblings2()
        {
            var document = Parse("div1 > div2 > child1 + child2");
            Assert.AreEqual(1, document.Children.Count);
            var parent = document.Children[0];
            Assert.AreEqual(1, parent.Children.Count);
            Assert.AreEqual(2, parent.Children[0].Children.Count);
            Assert.AreEqual("child1", parent.Children[0].Children[0].Name);
            Assert.AreEqual("child2", parent.Children[0].Children[1].Name);
        }

        [Test]
        public void StatementWithOneChild()
        {
            var document = Parse("div > span");
            Assert.AreEqual(1, document.Children.Count);
            Assert.AreEqual(1, document.Children[0].Children.Count);
            Assert.AreEqual("div", document.Children[0].Name);
            Assert.AreEqual("span", document.Children[0].Children[0].Name);
        }

        [Test]
        public void StatementWithNestedChildren()
        {
            var document = Parse("div > span > a");
            Assert.AreEqual(1, document.Children.Count);
            Assert.AreEqual(1, document.Children[0].Children.Count);
            Assert.AreEqual("div", document.Children[0].Name);
            Assert.AreEqual("span", document.Children[0].Children[0].Name);
            Assert.AreEqual("a", document.Children[0].Children[0].Children[0].Name);
        }

        public class IdTests
        {
            [TestCase("div", "sample-id")]
            public void ElementWithIdProducesBlockElementWithIdAttribute(string element, string id)
            {
                var document = Parse(string.Format("{0}#{1}", element, id));
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

        public class ClassTests
        {
            [TestCase("div", "sample-class")]
            public void ElementWithIdProducesBlockElementWithClassAttribute(string element, string @class)
            {
                var document = Parse(string.Format("{0}.{1}", element, @class));
                Assert.AreEqual("class", document.Children[0].Attributes[0].Key);
                Assert.IsInstanceOf<StringLiteral>(document.Children[0].Attributes[0].Value);
                Assert.AreEqual(@class, (document.Children[0].Attributes[0].Value as StringLiteral).Values[0].Data);
            }

            [TestCase("div", "class1", "class2", "class3")]
            public void ElementWithMultipleClassProducesBlockElementWithClassElementAndSpaceSeparatedClasses(string element, params string[] classes)
            {
                var document = Parse(string.Format("{0}.{1}", element, string.Join(".", classes)));
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
                var parts = new StringLiteral(string.Format("\"this {0}is awesome {0}right\"", delimiter)).Values;

                Assert.AreEqual(4, parts.Count);
                Assert.AreEqual(StringLiteralPartType.Literal, parts[0].Type);
                Assert.AreEqual(encoding, parts[1].Type);
                Assert.AreEqual(StringLiteralPartType.Literal, parts[2].Type);
                Assert.AreEqual(encoding, parts[1].Type);

                Assert.AreEqual("this ", parts[0].Data);
                Assert.AreEqual("is", parts[1].Data);
                Assert.AreEqual(" awesome ", parts[2].Data);
                Assert.AreEqual("right", parts[3].Data);

                parts = new StringLiteral(string.Format("\"this contains a {0} but not a keyword\"", delimiter)).Values;
                Assert.AreEqual(1, parts.Count);
                Assert.AreEqual(StringLiteralPartType.Literal, parts[0].Type);

                parts = new StringLiteral(string.Format("\"{0}keyword_only\"", delimiter)).Values;
                Assert.AreEqual(1, parts.Count);
                Assert.AreEqual(encoding, parts[0].Type);
                Assert.AreEqual("keyword_only", parts[0].Data);

                parts = new StringLiteral(string.Format("\"{0}keyword_first followed by more words\"", delimiter)).Values;
                Assert.AreEqual(2, parts.Count);
                Assert.AreEqual(encoding, parts[0].Type);
                Assert.AreEqual(StringLiteralPartType.Literal, parts[1].Type);

                parts = new StringLiteral(string.Format("\"{0}keyword.with.dot\"", delimiter)).Values;
                Assert.AreEqual(1, parts.Count);
                Assert.AreEqual(encoding, parts[0].Type);
                Assert.AreEqual("keyword.with.dot", parts[0].Data);

                parts = new StringLiteral(string.Format("\"this is an {0}{0} escaped colon\"", delimiter)).Values;
                Assert.AreEqual(1, parts.Count);
                Assert.AreEqual(string.Format("this is an {0} escaped colon", delimiter), parts[0].Data);

                parts = new StringLiteral(string.Format("\"{0}keyword_only_endsin. a dot\"", delimiter)).Values;
                Assert.AreEqual(2, parts.Count);
                Assert.AreEqual(encoding, parts[0].Type);
                Assert.AreEqual(StringLiteralPartType.Literal, parts[1].Type);
                Assert.AreEqual("keyword_only_endsin", parts[0].Data);
                Assert.AreEqual(". a dot", parts[1].Data);
            }
        }

        public class ParameterTests
        {
            [Test]
            public void ParameterLoadsOneParameter()
            {
                var document = Parse("div(param1)");
                Assert.AreEqual(1, document.Children[0].Parameters.Count);
            }

            [Test]
            public void ParameterLoadsTwoParameters()
            {
                var document = Parse("div(param1, param2)");
                Assert.AreEqual(2, document.Children[0].Parameters.Count);
            }
        }

        public class ColonTests
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

        public class SiblingTests
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

        public class StringLiteralTests
        {
            [Test]
            public void Blah()
            {
                var document = Parse("\"string\"\"with quote in it\"");
                Assert.AreEqual("string\"with quote in it", (document.Children[0] as StringLiteral).ToString());
            }

            [Test]
            public void StringLiteralPipeAsLastElementOfFile()
            {
                //TODO: Remove \r\n
                //This appears to be a bug in the gold engine
                //when converted to new lexer/parser remove
                //the \r\n
                var document = Parse("|string literal\r\n");
                Assert.AreEqual(1, document.Children.Count);
                Assert.AreEqual("string", document.Children[0].Name);
            }

            [Test]
            public void StringLiteralPipeChildFollowedByStringLiteralPipeChild()
            {
                var document = Parse(@"container { 
                                            style[type='text/css'] |label { margin-right: .5em; font-weight: bold; }
                                            title |Parrot Test Drive
                                       }");

                Assert.AreEqual(1, document.Children.Count);
                Assert.AreEqual(2, document.Children[0].Children.Count);
            }

            [Test]
            public void IdentifierPartsTests()
            {
                var results = GetIdentifierParts("input:submit#id.class").ToList();
                Assert.AreEqual(4, results.Count);

                Assert.AreEqual("input", results[0].Name);
                Assert.AreEqual(IdentifierType.Literal, results[0].Type);
                Assert.AreEqual("submit", results[1].Name);
                Assert.AreEqual(IdentifierType.Type, results[1].Type);
                Assert.AreEqual("id", results[2].Name);
                Assert.AreEqual(IdentifierType.Id, results[2].Type);
                Assert.AreEqual("class", results[3].Name);
                Assert.AreEqual(IdentifierType.Class, results[3].Type);
            }

            [Test]
            public void StringWithAtFollowedByEquals()
            {
                var document = Parse("'s=@Name'");
                Assert.IsNotNull(document);
            }

            public IEnumerable<Identifier> GetIdentifierParts(string source)
            {
                int index = 0;

                var partType = IdentifierType.Literal;

                for (int i = 0; i < source.Length; i++)
                {
                    switch (source[i])
                    {
                        case ':':
                            yield return new Identifier
                                {
                                    Name = source.Substring(index, i - index),
                                    Type = partType
                                };
                            partType = IdentifierType.Type;
                            index = i + 1;
                            break;
                        case '#':
                            yield return new Identifier
                                {
                                    Name = source.Substring(index, i - index),
                                    Type = partType
                                };
                            partType = IdentifierType.Id;
                            index = i + 1;
                            break;
                        case '.':
                            yield return new Identifier
                                {
                                    Name = source.Substring(index, i - index),
                                    Type = partType
                                };
                            partType = IdentifierType.Class;
                            index = i + 1;
                            break;
                    }
                }

                yield return new Identifier
                    {
                        Name = source.Substring(index),
                        Type = partType
                    };
            }
        }
    }

    public class IdentifierExtractionTests
    {
    }
}