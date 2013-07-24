namespace Parrot.Lexer
{
    internal class CommaToken : Token
    {
        public override TokenType Type
        {
            get { return TokenType.Comma; }
        }
    }
}