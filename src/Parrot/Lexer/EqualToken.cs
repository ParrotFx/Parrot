namespace Parrot.Lexer
{
    public class EqualToken : Token
    {
        public EqualToken()
        {
            Content = "=";
            Type = TokenType.Equal;
        }
    }
}