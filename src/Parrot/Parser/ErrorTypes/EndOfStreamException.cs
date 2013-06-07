namespace Parrot.Parser.ErrorTypes
{
    public class EndOfStreamException : ParserError
    {
        public override string Message
        {
            get { return "Unexpected end of file."; }
        }
    }
}