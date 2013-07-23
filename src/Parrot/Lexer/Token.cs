namespace Parrot.Lexer
{
    public class Token
    {
        public TokenType Type { get; protected set; }
        public string Content { get; set; }
        public int Index { get; set; }
    }
}