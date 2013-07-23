namespace Parrot.Lexer
{
    internal class OpenBracketToken : Token
    {
        public OpenBracketToken()
        {
            Content = "[";
        }

        public override TokenType Type
        {
            get { return TokenType.OpenBracket; }
        }
    }
}