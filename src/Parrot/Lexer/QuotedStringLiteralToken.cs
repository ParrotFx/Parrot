namespace Parrot.Lexer
{
    internal class QuotedStringLiteralToken : Token
    {
        public override TokenType Type
        {
            get { return TokenType.QuotedStringLiteral; }
        }
    }
}