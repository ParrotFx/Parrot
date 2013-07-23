namespace Parrot.Lexer
{
	internal class IdentifierToken : Token
	{
	    public override TokenType Type
	    {
	        get { return TokenType.Identifier; }
	    }
	}
}