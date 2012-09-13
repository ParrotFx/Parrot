namespace Parrot.Lexer
{
    public class CloseBracesToken : Token
    {
        public CloseBracesToken()
        {
            Content = "}";
            Type = TokenType.CloseBrace;
        }
    }
}