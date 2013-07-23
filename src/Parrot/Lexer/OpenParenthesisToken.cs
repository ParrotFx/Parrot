namespace Parrot.Lexer
{
    internal class OpenParenthesisToken : Token
    {
        public OpenParenthesisToken()
        {
            Content = "(";
        }

        public override TokenType Type
        {
            get { return TokenType.OpenParenthesis; }
        }
    }
}