// -----------------------------------------------------------------------
// <copyright file="ParrotParserTests.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using Parrot.Mvc.Renderers;

namespace Parrot.Tests
{
    using System.Linq;
    using Nodes;
    using Parrot;
    using NUnit.Framework;

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
        private Document Parse(string text)
        {
            Parser.Parser parsr = new Parser.Parser();
            Document document;

            parsr.Parse(text, out document);

            return document;
        }

        [Test]
        public void StringWithBlockNameReturnsBlockNodeWithProperName()
        {
            string block = "div";
            Document nodes = Parse(block);

            Assert.IsNotNull(nodes);
            Assert.AreEqual(block, nodes.Children.Single().BlockName);
        }

        [Test]
        public void StringWithInvalidStartCharacterReturnsNull()
        {
            var block = "1div";
            var nodes = Parse(block);

            Assert.IsNull(nodes);
        }

        [Test]
        public void BlockWithSingleAttributeHasAllRequiredStuff()
        {
            var block = "div[attr=\"value\"]";
            var nodes = Parse(block);

            Assert.AreEqual(1, nodes.Children.Single().Attributes.Count);
            Assert.AreEqual("attr", nodes.Children.Single().Attributes.Single().Key);
            Assert.AreEqual("value", nodes.Children.Single().Attributes.Single().Value);
        }

        [Test]
        public void MultipleAttributes()
        {
            var block = "div[attr0=\"value0\" attr1=\"value1\"]";
            var nodes = Parse(block);

            var blockNode = nodes.Children.Single();

            Assert.AreEqual(2, blockNode.Attributes.Count);
            Assert.AreEqual("attr0", blockNode.Attributes[0].Key);
            Assert.AreEqual("value0", blockNode.Attributes[0].Value);
            Assert.AreEqual(ValueType.StringLiteral, blockNode.Attributes[0].ValueType);

            Assert.AreEqual("attr1", blockNode.Attributes[1].Key);
            Assert.AreEqual("value1", blockNode.Attributes[1].Value);
        }

        [Test]
        public void BlockWithParameter()
        {
            var block = "div(parameter)";
            var nodes = Parse(block);

            var blockNode = nodes.Children.Single();

            Assert.AreEqual(1, blockNode.Parameters.Count);
            Assert.AreEqual("parameter", blockNode.Parameters[0].Value);
            Assert.AreEqual(ValueType.Property, blockNode.Parameters[0].ValueType);
        }

        [Test]
        public void BlockWithMultipleParameter()
        {
            var block = "div(parameter1 parameter2)";
            var nodes = Parse(block);

            var blockNode = nodes.Children.Single();

            Assert.AreEqual(2, blockNode.Parameters.Count);
            Assert.AreEqual("parameter1", blockNode.Parameters[0].Value);
            Assert.AreEqual(ValueType.Property, blockNode.Parameters[0].ValueType);

            Assert.AreEqual("parameter2", blockNode.Parameters[1].Value);
            Assert.AreEqual(ValueType.Property, blockNode.Parameters[1].ValueType);
        }

        //something to do with sequential elements
        [Test]
        public void TwoElements()
        {
            string block = "div div";
            var nodes = Parse(block);

            Assert.AreEqual(2, nodes.Children.Count);
            Assert.IsNotNull(nodes.Children.First());
            Assert.IsNotNull(nodes.Children.Last());
        }

        //something to do with sequential elements and children
        [Test]
        public void TwoElementsWithChildren()
        {
            string block = "div { span } div";
            var nodes = Parse(block);

            Assert.AreEqual(2, nodes.Children.Count);
            Assert.IsNotNull(nodes.Children.First());
            Assert.AreEqual(1, nodes.Children.First().Children.Count);
            Assert.IsNotNull(nodes.Children.Last());
        }

        //something to do with children
        [Test]
        public void Children()
        {
            string block = "div { span }";
            var nodes = Parse(block);

            Assert.AreEqual(1, nodes.Children.First().Children.Count);
            Assert.AreEqual("span", nodes.Children.First().Children.First().BlockName);
        }

        //Renderers
        [Test]
        public void FunctionStyleElement()
        {
            IRendererFactory factory = new RendererFactory();

            factory.RegisterFactory("div", new HtmlRenderer());

            string block = "div";
            var nodes = Parse(block);

            var renderer = factory.GetRenderer(block);

            var result = renderer.Render(nodes.Children.First(), null);

            Assert.AreEqual("<div></div>", result);
        }

        [Test]
        public void TestingHomeControllerDefaultTemplate()
        {
            var block = Parrot.SampleSite.Controllers.HomeController.DefaultHtmlTemplate;
            var nodes = Parse(block);

            Assert.AreEqual(5, nodes.Children.Count);
        }

        [Test]
        public void OutputTest()
        {
            var block = "div { =test }";
            var nodes = Parse(block);

            Assert.IsNotNull(nodes.Children.First());
            Assert.AreEqual(1, nodes.Children.First().Children.Count);
            Assert.AreEqual("test", (nodes.Children.First().Children.First() as OutputNode).VariableName);
        }

        [Test]
        public void RawOutputTest()
        {
            var block = "div { :test }";
            var nodes = Parse(block);

            Assert.IsNotNull(nodes.Children.First());
            Assert.AreEqual(1, nodes.Children.First().Children.Count);
            Assert.AreEqual("test", (nodes.Children.First().Children.First() as RawOutputNode).VariableName);
        }

        [Test]
        public void Classes()
        {
            var block = "div.test-class";
            var nodes = Parse(block);
            Assert.IsNotNull(nodes.Children.First());
            Assert.AreEqual(1, nodes.Children.First().Attributes.Count);
            Assert.AreEqual("class", nodes.Children.First().Attributes[0].Key);
            Assert.AreEqual("test-class", nodes.Children.First().Attributes[0].Value);
        }

        [Test]
        public void Ids()
        {
            var block = "div#sample-id";
            var nodes = Parse(block);
            Assert.AreEqual("id", nodes.Children.First().Attributes.First().Key);
            Assert.AreEqual("sample-id", nodes.Children.First().Attributes.First().Value);
        }

        [Test]
        public void ClassAndIds()
        {
            var block = "div#sample-id.test-class";
            var nodes = Parse(block);
            Assert.AreEqual("class", nodes.Children.First().Attributes.First().Key);
            Assert.AreEqual("test-class", nodes.Children.First().Attributes.First().Value);
            Assert.AreEqual("id", nodes.Children.First().Attributes.Last().Key);
            Assert.AreEqual("sample-id", nodes.Children.First().Attributes.Last().Value);
        }
    }

}
