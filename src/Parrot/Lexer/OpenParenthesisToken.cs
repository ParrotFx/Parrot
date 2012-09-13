namespace Parrot.Lexer
{
    internal class OpenParenthesisToken : Token
    {
        public OpenParenthesisToken()
        {
            Content = "(";
            Type = TokenType.OpenParenthesis;
        }
    }
}