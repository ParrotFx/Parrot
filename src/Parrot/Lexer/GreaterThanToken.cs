namespace Parrot.Lexer
{
    internal class GreaterThanToken : Token
    {
        public GreaterThanToken()
        {
            Content = ">";
        }

        public override TokenType Type
        {
            get { return TokenType.GreaterThan; }
        }
    }
}