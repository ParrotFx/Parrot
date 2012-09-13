namespace Parrot.Lexer
{
    internal class GreaterThanToken : Token
    {
        public GreaterThanToken()
        {
            Content = ">";
            Type = TokenType.GreaterThan;
        }
    }
}