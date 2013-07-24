namespace Parrot.Tests.Parser
{
    using NUnit.Framework;

    [TestFixture]
    public class ParameterTests : ParrotParserTestsBase
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
}