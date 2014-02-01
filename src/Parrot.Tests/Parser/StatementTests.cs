// -----------------------------------------------------------------------
// <copyright file="StatementTests.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Parrot.Tests.Parser
{
    using NUnit.Framework;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [TestFixture]
    public class StatementTests : ParrotParserTestsBase
    {
        //so what are we tsting
        //block name == blcok name
        //block followed by block
        //block followed by ; with block
        //attribute name/values
        //parameter name/values

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
    }
}