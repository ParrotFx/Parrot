namespace Parrot.Lexer
{
    internal class PlusToken : Token
    {
        public override TokenType Type
        {
            get { return TokenType.Plus; }
        }
    }
}