namespace Parrot.Lexer
{
    public class CloseBracesToken : Token
    {
        public override TokenType Type
        {
            get { return TokenType.CloseBrace; }
        }
    }
}