namespace Parrot.Lexer
{
    public abstract class Token
    {
        public abstract TokenType Type { get; }
        public string Content { get; set; }
        public int Index { get; set; }

        public override string ToString()
        {
            return string.Format("{0} ({1})", Content, Type);
        }
    }
}