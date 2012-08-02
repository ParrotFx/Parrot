using Parrot.Nodes;

namespace Parrot.Parser
{
    // this class will construct a parser without having to process
    //  the CGT tables with each creation.  It must be initialized
    //  before you can call CreateParser()

    public class ParserContext
    {

        // instance fields
        private readonly GoldParser.Parser _parser;

        // constructor
        public ParserContext(GoldParser.Parser parser)
        {
            _parser = parser;
        }


        public string GetTokenText()
        {
            // delete any of these that are non-terminals.

            switch (_parser.TokenSymbol.Index)
            {
                case (int)SymbolConstants.SYMBOL_EOF:
                case (int)SymbolConstants.SYMBOL_WHITESPACE:
                case (int)SymbolConstants.SYMBOL_LPARAN:
                case (int)SymbolConstants.SYMBOL_RPARAN:
                case (int)SymbolConstants.SYMBOL_LBRACKET:
                case (int)SymbolConstants.SYMBOL_RBRACKET:
                case (int)SymbolConstants.SYMBOL_LBRACE:
                case (int)SymbolConstants.SYMBOL_SEMI:
                case (int)SymbolConstants.SYMBOL_RBRACE:
                case (int)SymbolConstants.SYMBOL_EQ:
                case (int)SymbolConstants.SYMBOL_MULTILINESTRINGLITERAL:
                case (int)SymbolConstants.SYMBOL_STRINGLITERAL:
                case (int)SymbolConstants.SYMBOL_ATTRIBUTE:
                case (int)SymbolConstants.SYMBOL_IDENTIFIER:
                case (int)SymbolConstants.SYMBOL_HTMLIDENTIFIER:
                case (int)SymbolConstants.SYMBOL_CLASSIDENTIFIER:
                case (int)SymbolConstants.SYMBOL_STRINGLITERALPIPE:
                    return _parser.TokenString;
                default:
                    return null;
            }

        }

        public AbstractNode CreateASTNode()
        {
            AttributeNodeList idClassAttributes;
            AttributeNodeList attributes;
            ParameterNodeList parameter;
            BlockNodeList children;

            switch (_parser.ReductionRule.Index)
            {
                case (int)RuleConstants.RULE_PARAMETERS_LPARAN_RPARAN:
                    // <Parameters> ::= '(' <Parameter List> ')'
                    var parameters = _parser.GetReductionSyntaxNode(1) as ParameterNodeList;
                    if (parameters == null)
                    {
                        parameters = new ParameterNodeList(_parser.GetReductionSyntaxNode(1) as ParameterNode);
                    }

                    return parameters;

                case (int)RuleConstants.RULE_PARAMETER_STRINGLITERAL:
                case (int)RuleConstants.RULE_PARAMETER_IDENTIFIER:
                    //<Parameter> ::= StringLiteral
                    //<Parameter> ::= Identifier
                    return new ParameterNode(Token(0));

                case (int)RuleConstants.RULE_PARAMETERLIST:
                    // <Parameter List> ::= <Parameter>
                    if (_parser.ReductionCount > 0)
                    {
                        return new ParameterNodeList(new ParameterNode(Token(0)));
                    }

                    return null;

                case (int)RuleConstants.RULE_PARAMETERLIST2:
                    // <Parameter List> ::= <Parameter List> <Parameter>
                    var parameterList = _parser.GetReductionSyntaxNode(0) as ParameterNodeList;
                    if (parameterList == null)
                    {
                        parameterList = new ParameterNodeList(_parser.GetReductionSyntaxNode(0) as ParameterNode);
                    }

                    return new ParameterNodeList(parameterList, _parser.GetReductionSyntaxNode(1) as ParameterNode);

                case (int)RuleConstants.RULE_PARAMETERS:
                    //<Parameter> ::= 
                    //empty parameter list
                    return null;

                case (int)RuleConstants.RULE_ATTRIBUTE_IDENTIFIER:
                    // <Attribute> ::= Identifier
                    return new AttributeNode(Token(0), null);

                case (int)RuleConstants.RULE_ATTRIBUTE_IDENTIFIER_EQ_STRINGLITERAL:
                    //<Attribute> ::= Identifier '=' StringLiteral
                    return new AttributeNode(Token(0), Token(2));

                case (int)RuleConstants.RULE_ATTRIBUTE_IDENTIFIER_EQ_IDENTIFIER:
                    //<Attribute> ::= Identifier '=' Identifier
                    return new AttributeNode(Token(0), Token(2));

                case (int)RuleConstants.RULE_ATTRIBUTELIST:
                    //<Attribute List> ::= <Attribute>

                    if (_parser.ReductionCount > 0)
                    {
                        return new AttributeNodeList(_parser.GetReductionSyntaxNode(0) as AttributeNode);
                    }

                    return null;

                case (int)RuleConstants.RULE_ATTRIBUTELIST2:
                    //<Attribute List> ::= <Attribute List> ',' <Attribute>

                    var attributeList = _parser.GetReductionSyntaxNode(0) as AttributeNodeList;
                    if (attributeList == null)
                    {
                        attributeList = new AttributeNodeList(_parser.GetReductionSyntaxNode(0) as AttributeNode);
                    }

                    return new AttributeNodeList(attributeList, _parser.GetReductionSyntaxNode(1) as AttributeNode);

                case (int)RuleConstants.RULE_ATTRIBUTES_LBRACKET_RBRACKET:
                    //<Attributes> ::= '[' <Attribute List> ']'
                    var attributeNodeList = _parser.GetReductionSyntaxNode(1) as AttributeNodeList;
                    if (attributeNodeList == null)
                    {
                        attributeNodeList = new AttributeNodeList(_parser.GetReductionSyntaxNode(1) as AttributeNode);
                    }

                    return attributeNodeList;

                case (int)RuleConstants.RULE_ATTRIBUTES:
                    //<Attributes> ::= 
                    // empty attribute list
                    return null;

                case (int)RuleConstants.RULE_CLASSES_CLASSIDENTIFIER:
                    //<Classes> ::= ClassIdentifier
                    return new AttributeNodeList(new AttributeNode("class", Token(0).Substring(1)));

                case (int)RuleConstants.RULE_CLASSES_CLASSIDENTIFIER2:
                    //<Classes> ::= ClassIdentifier <Classes>
                    return new AttributeNodeList(_parser.GetReductionSyntaxNode(1) as AttributeNodeList, new AttributeNode("class", Token(0).Substring(1)));

                case (int)RuleConstants.RULE_IDCLASS_HTMLIDENTIFIER:
                    //<IdClass> ::= HtmlIdentifier
                    if (_parser.ReductionCount > 0)
                    {
                        var id = Token(0);
                        if (id.StartsWith("#"))
                        {
                            id = id.Substring(1);
                        }
                        return new AttributeNodeList(new AttributeNode("id", id));
                    }

                    return null;

                case (int)RuleConstants.RULE_IDCLASS_HTMLIDENTIFIER2:
                    //<IdClass> ::= HtmlIdentifier <Classes>
                    return new AttributeNodeList(_parser.GetReductionSyntaxNode(1) as AttributeNodeList, new AttributeNode("id", Token(0).Substring(1)));

                case (int)RuleConstants.RULE_IDCLASS:
                    //<IdClass> ::= <Classes>
                    return new AttributeNodeList(_parser.GetReductionSyntaxNode(0) as AttributeNodeList);

                case (int)RuleConstants.RULE_IDCLASS2:
                    //<IdClass> ::= 
                    // empty id class
                    return null;

                #region " Statements "

                case (int)RuleConstants.RULE_STATEMENTS:
                    {
                        //<Statements> ::= <Statement>
                        return new BlockNodeList(new[] { _parser.GetReductionSyntaxNode(0) as BlockNode });
                    }

                case (int)RuleConstants.RULE_STATEMENTS2:
                    {
                        //<Statements> ::= <Statements> <Statement> 
                        var elements = _parser.GetReductionSyntaxNode(0) as BlockNodeList;
                        if (elements == null)
                        {
                            elements = new BlockNodeList(_parser.GetReductionSyntaxNode(0) as BlockNode);
                        }

                        return new BlockNodeList(elements, _parser.GetReductionSyntaxNode(1) as BlockNode);
                    }

                case (int)RuleConstants.RULE_STATEMENT_IDENTIFIER_SEMI:
                    {
                        // <Statement> ::= Identifier <IdClass> <Attributes> <Parameter> ':' <Statement>
                        idClassAttributes = _parser.GetReductionSyntaxNode(1) as AttributeNodeList;
                        attributes = _parser.GetReductionSyntaxNode(2) as AttributeNodeList;
                        parameter = _parser.GetReductionSyntaxNode(3) as ParameterNodeList;
                        children = _parser.GetReductionSyntaxNode(5) as BlockNodeList;
                        if (children == null)
                        {
                            children = new BlockNodeList(_parser.GetReductionSyntaxNode(5) as BlockNode);
                        }

                        return new BlockNode(
                            Token(0),
                            MergeAttributeNodes(attributes, idClassAttributes),
                            parameter,
                            children
                        );
                    }

                case (int)RuleConstants.RULE_STATEMENT_STRINGLITERALPIPE:
                    return new StringLiteralNode(Token(1));

                case (int)RuleConstants.RULE_STATEMENT_IDENTIFIER_LBRACE_RBRACE:
                    //<Statement> ::= Identifier <IdClass> <Attributes> <Parameter> '{' <Statements> '}'

                    idClassAttributes = _parser.GetReductionSyntaxNode(1) as AttributeNodeList;
                    attributes = _parser.GetReductionSyntaxNode(2) as AttributeNodeList;
                    parameter = _parser.GetReductionSyntaxNode(3) as ParameterNodeList;
                    children = _parser.GetReductionSyntaxNode(5) as BlockNodeList;
                    if (children == null)
                    {
                        children = new BlockNodeList(_parser.GetReductionSyntaxNode(5) as BlockNode);
                    }

                    return new BlockNode(
                        Token(0),
                        MergeAttributeNodes(attributes, idClassAttributes),
                        parameter,
                        children
                    );

                case (int)RuleConstants.RULE_STATEMENT_STRINGLITERAL:
                case (int)RuleConstants.RULE_STATEMENT_MULTILINESTRINGLITERAL:
                    //<Statement> ::= StringLiteral
                    //<Statement> ::= MultiLineStringLiteral
                    return new StringLiteralNode(Token(0));

                case (int)RuleConstants.RULE_STATEMENT_IDENTIFIER_LBRACE_RBRACE2:
                    // <Statement> ::= Identifier <IdClass> <Attributes> <Parameters> '{' '}'
                    return new BlockNode(
                        Token(0),
                        MergeAttributeNodes(_parser.GetReductionSyntaxNode(1) as AttributeNodeList, _parser.GetReductionSyntaxNode(2) as AttributeNodeList),
                        _parser.GetReductionSyntaxNode(3) as ParameterNodeList,
                        _parser.GetReductionSyntaxNode(5) as BlockNodeList
                    );

                case (int)RuleConstants.RULE_STATEMENT_IDENTIFIER:
                    // <Statement> ::= Identifier EndOfLineOrFile
                    return new BlockNode(
                        Token(0),
                        MergeAttributeNodes(_parser.GetReductionSyntaxNode(1) as AttributeNodeList, _parser.GetReductionSyntaxNode(2) as AttributeNodeList),
                        _parser.GetReductionSyntaxNode(3) as ParameterNodeList
                    );

                case (int)RuleConstants.RULE_OUTPUTSTATEMENT_COLON_IDENTIFIER:
                    // <OutputStatement> ::= ':' Identifier
                    return new RawOutputNode(Token(1));

                case (int)RuleConstants.RULE_OUTPUTSTATEMENT_EQ_IDENTIFIER:
                    // <OutputStatement> ::= '=' Identifier
                    return new OutputNode(Token(1));

                #endregion

                default:
                    throw new RuleException("Unknown rule: Does your CGT Match your Code Revision?");
            }

        }

        private AttributeNodeList MergeAttributeNodes(AttributeNodeList destinationNodes, AttributeNodeList sourceNodes)
        {
            if (sourceNodes != null && destinationNodes != null)
            {
                foreach (var source in sourceNodes)
                {
                    bool found = false;
                    foreach (var destination in destinationNodes)
                    {
                        if (destination.Key == source.Key)
                        {
                            found = true;
                            //override or append?
                            destination.Key = source.Key;
                            destination.ValueType = source.ValueType;
                            destination.Value = source.Value;
                        }
                    }

                    if (!found)
                    {
                        destinationNodes.Add(source);
                    }
                }
            }

            return destinationNodes ?? sourceNodes;
        }

        private string Token(int index)
        {
            return (string)_parser.GetReductionSyntaxNode(index);
        }
    }
}
