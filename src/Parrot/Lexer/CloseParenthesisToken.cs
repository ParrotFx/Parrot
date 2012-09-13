namespace Parrot.Lexer
{
    public class CloseParenthesisToken : Token
    {
        public CloseParenthesisToken()
        {
            Content = ")";
            Type = TokenType.CloseParenthesis;
        }
    }
}