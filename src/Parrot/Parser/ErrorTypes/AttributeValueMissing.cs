namespace Parrot.Parser.ErrorTypes
{
    public class AttributeValueMissing : ParserError
    {
        public override string Message
        {
            get { return "Attribute must have a value"; }
        }
    }
}