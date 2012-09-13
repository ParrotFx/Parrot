namespace Parrot.Lexer
{
    internal class CommaToken : Token
    {
        public CommaToken()
        {
            Content = ",";
            Type = TokenType.Comma;
        }
    }
}