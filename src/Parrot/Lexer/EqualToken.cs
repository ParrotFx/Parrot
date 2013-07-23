namespace Parrot.Lexer
{
    public class EqualToken : Token
    {
        public override TokenType Type
        {
            get { return TokenType.Equal; }
        }
    }
}