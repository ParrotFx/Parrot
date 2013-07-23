namespace Parrot.Lexer
{
    internal class StringLiteralPipeToken : Token
    {
		public StringLiteralPipeToken()
		{
			Type = TokenType.StringLiteralPipe;
		}
    }
}