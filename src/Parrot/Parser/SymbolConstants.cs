namespace Parrot.Parser
{
    enum SymbolConstants : int
    {
        SYMBOL_EOF = 0, // (EOF)
        SYMBOL_ERROR = 1, // (Error)
        SYMBOL_WHITESPACE = 2, // Whitespace
        SYMBOL_LPARAN = 3, // '('
        SYMBOL_RPARAN = 4, // ')'
        SYMBOL_COLON = 5, // ':'
        SYMBOL_SEMI = 6, // ';'
        SYMBOL_LBRACKET = 7, // '['
        SYMBOL_RBRACKET = 8, // ']'
        SYMBOL_LBRACE = 9, // '{'
        SYMBOL_RBRACE = 10, // '}'
        SYMBOL_EQ = 11, // '='
        SYMBOL_CLASSIDENTIFIER = 12, // ClassIdentifier
        SYMBOL_HTMLIDENTIFIER = 13, // HtmlIdentifier
        SYMBOL_IDENTIFIER = 14, // Identifier
        SYMBOL_MULTILINESTRINGLITERAL = 15, // MultiLineStringLiteral
        SYMBOL_STRINGLITERAL = 16, // StringLiteral
        SYMBOL_STRINGLITERALPIPE = 17, // StringLiteralPipe
        SYMBOL_ATTRIBUTE = 18, // <Attribute>
        SYMBOL_ATTRIBUTELIST = 19, // <Attribute List>
        SYMBOL_ATTRIBUTES = 20, // <Attributes>
        SYMBOL_CLASSES = 21, // <Classes>
        SYMBOL_IDCLASS = 22, // <IdClass>
        SYMBOL_OUTPUTSTATEMENT = 23, // <OutputStatement>
        SYMBOL_PARAMETER = 24, // <Parameter>
        SYMBOL_PARAMETERLIST = 25, // <Parameter List>
        SYMBOL_PARAMETERS = 26, // <Parameters>
        SYMBOL_STATEMENT = 27, // <Statement>
        SYMBOL_STATEMENTS = 28  // <Statements>
    };
}