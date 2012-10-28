namespace Parrot.Lexer
{
    internal class AtToken : Token
    {
        public AtToken()
        {
            Content = "@";
            Type = TokenType.At;
        }
    }
}