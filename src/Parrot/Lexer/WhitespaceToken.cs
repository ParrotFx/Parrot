namespace Parrot.Lexer
{
    internal class WhitespaceToken : Token
    {
        public override TokenType Type
        {
            get { return TokenType.Whitespace; }
        }
    }
}