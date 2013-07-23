namespace Parrot.Lexer
{
    internal class AtToken : Token
    {
        public AtToken()
        {
            Content = "@";
        }

        public override TokenType Type
        {
            get { return TokenType.At; }
        }
    }
}