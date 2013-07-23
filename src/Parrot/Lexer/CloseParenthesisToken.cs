namespace Parrot.Lexer
{
    public class CloseParenthesisToken : Token
    {
        public CloseParenthesisToken()
        {
            Content = ")";
        }

        public override TokenType Type
        {
            get { return TokenType.CloseParenthesis; }
        }
    }
}