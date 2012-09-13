namespace Parrot.Lexer
{
    public enum TokenType : int
    {
        Start = 1,
        Identifier = 2,
        QuotedStringLiteral,
        StringLiteral,
        OpenBracket,
        CloseBracket,
        OpenParenthesis,
        CloseParenthesis,
        Comma,
        OpenBrace,
        CloseBrace,
        GreaterThan,
        Plus,
        Whitespace,
        StringLiteralPipe,
        MultiLineStringLiteral,
        CommentLine,
        CommentStart,
        CommentEnd,
        Colon,
        Equal
    }
}