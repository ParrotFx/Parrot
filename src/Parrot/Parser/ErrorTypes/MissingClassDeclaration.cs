namespace Parrot.Parser.ErrorTypes
{
    public class MissingClassDeclaration : ParserError
    {
        public override string Message
        {
            get { return "Missing class declaration"; }
        }
    }
}