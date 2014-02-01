namespace Parrot.Lexer
{
    internal class CaretToken : Token
    {
        public override TokenType Type
        {
            get { return TokenType.Caret; }
        }
    }
}
