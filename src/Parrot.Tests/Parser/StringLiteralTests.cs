namespace Parrot.Tests.Parser
{
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;
    using Parrot.Nodes;

    [TestFixture]
    public class StringLiteralTests : ParrotParserTestsBase
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