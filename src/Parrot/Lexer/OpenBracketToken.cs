namespace Parrot.Lexer
{
    internal class OpenBracketToken : Token
    {
        public override TokenType Type
        {
            get { return TokenType.OpenBracket; }
        }
    }
}