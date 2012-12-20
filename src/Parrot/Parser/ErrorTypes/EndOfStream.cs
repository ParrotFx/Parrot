namespace Parrot.Parser.ErrorTypes
{
    public class EndOfStream : ParserError
    {
        public override string Message { get { return "Unexpected end of file."; } }
    }
}