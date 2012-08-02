namespace Parrot.Parser
{
    enum RuleConstants : int
    {
        RULE_PARAMETER_STRINGLITERAL = 0, // <Parameter> ::= StringLiteral
        RULE_PARAMETER_IDENTIFIER = 1, // <Parameter> ::= Identifier
        RULE_PARAMETERLIST = 2, // <Parameter List> ::= <Parameter>
        RULE_PARAMETERLIST2 = 3, // <Parameter List> ::= <Parameter List> <Parameter>
        RULE_PARAMETERS_LPARAN_RPARAN = 4, // <Parameters> ::= '(' <Parameter List> ')'
        RULE_PARAMETERS = 5, // <Parameters> ::= 
        RULE_ATTRIBUTE_IDENTIFIER_EQ_STRINGLITERAL = 6, // <Attribute> ::= Identifier '=' StringLiteral
        RULE_ATTRIBUTE_IDENTIFIER_EQ_IDENTIFIER = 7, // <Attribute> ::= Identifier '=' Identifier
        RULE_ATTRIBUTE_IDENTIFIER = 8, // <Attribute> ::= Identifier
        RULE_ATTRIBUTELIST = 9, // <Attribute List> ::= <Attribute>
        RULE_ATTRIBUTELIST2 = 10, // <Attribute List> ::= <Attribute List> <Attribute>
        RULE_ATTRIBUTES_LBRACKET_RBRACKET = 11, // <Attributes> ::= '[' <Attribute List> ']'
        RULE_ATTRIBUTES = 12, // <Attributes> ::= 
        RULE_CLASSES_CLASSIDENTIFIER = 13, // <Classes> ::= ClassIdentifier
        RULE_CLASSES_CLASSIDENTIFIER2 = 14, // <Classes> ::= ClassIdentifier <Classes>
        RULE_IDCLASS_HTMLIDENTIFIER = 15, // <IdClass> ::= HtmlIdentifier
        RULE_IDCLASS_HTMLIDENTIFIER2 = 16, // <IdClass> ::= HtmlIdentifier <Classes>
        RULE_IDCLASS = 17, // <IdClass> ::= <Classes>
        RULE_IDCLASS2 = 18, // <IdClass> ::= 
        RULE_STATEMENTS = 19, // <Statements> ::= <Statement>
        RULE_STATEMENTS2 = 20, // <Statements> ::= <Statements> <Statement>
        RULE_STATEMENT_IDENTIFIER_LBRACE_RBRACE = 21, // <Statement> ::= Identifier <IdClass> <Attributes> <Parameters> '{' <Statements> '}'
        RULE_STATEMENT_IDENTIFIER_LBRACE_RBRACE2 = 22, // <Statement> ::= Identifier <IdClass> <Attributes> <Parameters> '{' '}'
        RULE_STATEMENT_IDENTIFIER_SEMI = 23, // <Statement> ::= Identifier <IdClass> <Attributes> <Parameters> ';' <Statement>
        RULE_STATEMENT = 24, // <Statement> ::= <OutputStatement>
        RULE_STATEMENT_IDENTIFIER = 25, // <Statement> ::= Identifier <IdClass> <Attributes> <Parameters>
        RULE_STATEMENT_STRINGLITERALPIPE = 26, // <Statement> ::= StringLiteralPipe
        RULE_STATEMENT_MULTILINESTRINGLITERAL = 27, // <Statement> ::= MultiLineStringLiteral
        RULE_STATEMENT_STRINGLITERAL = 28, // <Statement> ::= StringLiteral
        RULE_OUTPUTSTATEMENT_COLON_IDENTIFIER = 29, // <OutputStatement> ::= ':' Identifier
        RULE_OUTPUTSTATEMENT_EQ_IDENTIFIER = 30  // <OutputStatement> ::= '=' Identifier
    };
}