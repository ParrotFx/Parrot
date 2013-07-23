namespace Parrot.Lexer
{
    internal class GreaterThanToken : Token
    {
        public override TokenType Type
        {
            get { return TokenType.GreaterThan; }
        }
    }
}