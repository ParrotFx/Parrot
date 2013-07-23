namespace Parrot.Lexer
{
    internal class StringLiteralPipeToken : Token
    {
        public override TokenType Type
        {
            get { return TokenType.StringLiteralPipe; }
        }
    }
}