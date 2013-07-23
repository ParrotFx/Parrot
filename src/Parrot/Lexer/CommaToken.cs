namespace Parrot.Lexer
{
    internal class CommaToken : Token
    {
        public CommaToken()
        {
            Content = ",";
        }

        public override TokenType Type
        {
            get { return TokenType.Comma; }
        }
    }
}