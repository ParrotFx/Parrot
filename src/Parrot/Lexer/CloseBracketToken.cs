namespace Parrot.Lexer
{
    public class CloseBracketToken : Token
    {
        public CloseBracketToken()
        {
            Content = "]";
        }

        public override TokenType Type
        {
            get { return TokenType.CloseBracket; }
        }
    }
}