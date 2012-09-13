namespace Parrot.Lexer
{
    internal class OpenBracketToken : Token
    {
        public OpenBracketToken()
        {
            Content = "[";
            Type = TokenType.OpenBracket;
        }
    }
}