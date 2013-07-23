namespace Parrot.Tests.Parser
{
    using Parrot.Nodes;

    public class ParrotParserTestsBase
    {
        public static Document Parse(string text)
        {
            Parrot.Parser.Parser parser = new Parrot.Parser.Parser();
            Document document;

            parser.Parse(text, out document);

            return document;
        }
    }
}