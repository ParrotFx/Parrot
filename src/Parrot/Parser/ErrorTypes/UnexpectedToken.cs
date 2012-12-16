using Parrot.Lexer;

namespace Parrot.Parser.ErrorTypes
{
    public class UnexpectedToken : ParserError
    {
        public override string Message { get { return ""; } }
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