namespace Parrot.Lexer
{
    internal class WhitespaceToken : Token
    {
        public WhitespaceToken()
        {
            Type = TokenType.Whitespace;
        }
    }
}