namespace Parrot.Lexer
{
    internal class OpenBracesToken : Token
    {
        public override TokenType Type
        {
            get { return TokenType.OpenBrace; }
        }
    }
}