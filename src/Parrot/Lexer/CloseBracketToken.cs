namespace Parrot.Lexer
{
    public class CloseBracketToken : Token
    {
        public override TokenType Type
        {
            get { return TokenType.CloseBracket; }
        }
    }
}