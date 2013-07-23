namespace Parrot.Lexer
{
	internal class IdentifierToken : Token
	{
		public IdentifierToken()
		{
			Type = TokenType.Identifier;
		}
	}
}