namespace Parrot.Lexer
{
    internal class ColonToken : Token
    {
        public ColonToken()
        {
            Content = ":";
            Type = TokenType.Colon;
        }
    }
}