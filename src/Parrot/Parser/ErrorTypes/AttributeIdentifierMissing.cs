namespace Parrot.Parser.ErrorTypes
{
    public class AttributeIdentifierMissing : ParserError
    {
        public override string Message
        {
            get { return "Invalid attribute name"; }
        }
    }
}