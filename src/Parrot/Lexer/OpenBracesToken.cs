namespace Parrot.Lexer
{
    internal class OpenBracesToken : Token
    {
        public OpenBracesToken()
        {
            Content = "{";
            Type = TokenType.OpenBrace;
        }
    }
}