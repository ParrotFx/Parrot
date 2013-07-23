namespace Parrot.Lexer
{
    internal class PlusToken : Token
    {
        public PlusToken()
        {
            Content = "+";
        }

        public override TokenType Type
        {
            get { return TokenType.Plus; }
        }
    }
}