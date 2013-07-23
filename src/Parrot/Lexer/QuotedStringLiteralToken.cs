namespace Parrot.Lexer
{
    internal class QuotedStringLiteralToken : Token
    {
		public QuotedStringLiteralToken()
		{
			Type = TokenType.QuotedStringLiteral;
		}
    }
}