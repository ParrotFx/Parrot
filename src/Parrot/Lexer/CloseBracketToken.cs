namespace Parrot.Lexer
{
    public class CloseBracketToken : Token
    {
        public CloseBracketToken()
        {
            Content = "]";
            Type = TokenType.CloseBracket;
        }
    }
}