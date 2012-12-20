namespace Parrot.Parser.ErrorTypes
{
    public class MissingIdDeclaration : ParserError
    {
        public override string Message { get { return "Missing Id declaration"; } }
    }
}