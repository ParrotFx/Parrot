namespace Parrot.Lexer
{
    public abstract class Token
    {
        public abstract TokenType Type { get; }
        public string Content { get; set; }
        public int Index { get; set; }
    }
}