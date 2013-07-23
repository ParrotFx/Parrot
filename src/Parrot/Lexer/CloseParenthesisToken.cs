namespace Parrot.Lexer
{
    public class CloseParenthesisToken : Token
    {
        public override TokenType Type
        {
            get { return TokenType.CloseParenthesis; }
        }
    }
}