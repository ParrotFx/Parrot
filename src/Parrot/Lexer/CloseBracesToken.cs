namespace Parrot.Lexer
{
    public class CloseBracesToken : Token
    {
        public CloseBracesToken()
        {
            Content = "}";
        }

        public override TokenType Type
        {
            get { return TokenType.CloseBrace; }
        }
    }
}