namespace Parrot.Lexer
{
    internal class PlusToken : Token
    {
        public PlusToken()
        {
            Content = "+";
            Type = TokenType.Plus;
        }
    }
}