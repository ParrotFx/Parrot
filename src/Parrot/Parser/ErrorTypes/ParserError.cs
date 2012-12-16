namespace Parrot.Parser.ErrorTypes
{
    public abstract class ParserError
    {
        public abstract string Message { get; }
        public int Index { get; set; }
        public int Length { get; set; }
    }
}