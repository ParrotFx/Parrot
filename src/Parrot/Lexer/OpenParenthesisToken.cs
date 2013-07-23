namespace Parrot.Lexer
{
    internal class OpenParenthesisToken : Token
    {
        public override TokenType Type
        {
            get { return TokenType.OpenParenthesis; }
        }
    }
}