namespace Parrot.Lexer
{
    internal class OpenBracesToken : Token
    {
        public OpenBracesToken()
        {
            Content = "{";
        }

        public override TokenType Type
        {
            get { return TokenType.OpenBrace; }
        }
    }
}