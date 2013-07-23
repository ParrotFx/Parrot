namespace Parrot.Lexer
{
    internal class AtToken : Token
    {
        public override TokenType Type
        {
            get { return TokenType.At; }
        }
    }
}