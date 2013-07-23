namespace Parrot.Lexer
{
    public class EqualToken : Token
    {
        public EqualToken()
        {
            Content = "=";
        }

        public override TokenType Type
        {
            get { return TokenType.Equal; }
        }
    }
}