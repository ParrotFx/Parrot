namespace Parrot.Parser.ErrorTypes
{
    using Parrot.Lexer;

    public class UnexpectedToken : ParserError
    {
        public override string Message
        {
            get { return string.Format("Unexpected token: {0}", Type.ToString()); }
        }

        protected TokenType Type { get; set; }
        protected string Token { get; set; }

        public UnexpectedToken(Token token)
        {
            Index = token.Index;
            Token = token.Content;
            Type = token.Type;
        }
    }
}