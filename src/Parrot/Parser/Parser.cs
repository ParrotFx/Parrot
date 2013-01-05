// -----------------------------------------------------------------------
// <copyright file="Parser.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------


namespace Parrot.Parser
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Parrot.Lexer;
    using Parrot.Nodes;
    using Parrot.Parser.ErrorTypes;

    public class Parser
    {
        public IList<ParserError> Errors { get; private set; }

        public Parser()
        {
            Errors = new List<ParserError>();
        }

        public bool Parse(System.IO.Stream stream, out Document document)
        {
            document = new Document();

            try
            {
                var tokenizer = new Tokenizer(stream);

                var tokens = tokenizer.Tokens();

                var tokenStream = new Stream(tokens);

                foreach (var statement in Parse(tokenStream))
                {
                    foreach (var s in statement)
                    {
                        ParseStatementErrors(s);
                        document.Children.Add(s);
                    }
                }
            }
            catch (EndOfStreamException)
            {
                Errors.Add(new EndOfStream {Index = (int) stream.Length});
            }

            document.Errors = Errors;

            return true;
        }

        private void ParseStatementErrors(Statement s)
        {
            if (s.Errors.Any())
            {
                foreach (var error in s.Errors)
                {
                    error.Index += s.Index;
                    Errors.Add(error);
                }
            }
        }

        public bool Parse(string text, out Document document)
        {
            return Parse(new MemoryStream(System.Text.Encoding.Default.GetBytes(text)), out document);
        }

        private IEnumerable<StatementList> Parse(Stream stream)
        {
            while (stream.Peek() != null)
            {
                var token = stream.Peek();
                switch (token.Type)
                {
                    case TokenType.StringLiteral:
                    case TokenType.StringLiteralPipe:
                    case TokenType.QuotedStringLiteral:
                    case TokenType.Identifier:
                    case TokenType.OpenBracket:
                    case TokenType.OpenParenthesis:
                    case TokenType.Equal:
                    case TokenType.At:
                        var statement = ParseStatement(stream);
                        yield return statement;
                        break;
                    default:
                        Errors.Add(new UnexpectedToken(token));
                        stream.Next();
                        break;
                        //throw new ParserException(token);
                }
            }
        }

        /// <summary>
        /// Parses a token stream creating the largest statement possible
        /// </summary>
        /// <param name="stream">Stream of tokens to parse</param>
        /// <returns>Statement</returns>
        private StatementList ParseStatement(Stream stream)
        {
            var previousToken = stream.Peek();
            if (previousToken == null)
            {
                Errors.Add(new EndOfStream());
                return new StatementList();
            }

            var tokenType = previousToken.Type;
            Token identifier = null;
            switch (tokenType)
            {
                case TokenType.Identifier:
                    //standard identifier
                    identifier = stream.Next();
                    break;
                case TokenType.OpenBracket:
                case TokenType.OpenParenthesis:
                    //ignore these
                    break;
                case TokenType.StringLiteral:
                case TokenType.StringLiteralPipe:
                case TokenType.QuotedStringLiteral:
                    //string statement
                    identifier = stream.Next();
                    break;
                case TokenType.At:
                    stream.GetNextNoReturn();
                    identifier = stream.Next();
                    break;
                case TokenType.Equal:
                    stream.GetNextNoReturn();
                    identifier = stream.Next();
                    break;
                default:
                    Errors.Add(new UnexpectedToken(previousToken));
                    return new StatementList();
                    //throw new ParserException(stream.Peek());
            }


            Statement statement;
            StatementTail tail = null;

            while (stream.Peek() != null)
            {
                var token = stream.Peek();
                if (token == null)
                {
                    break;
                }

                switch (token.Type)
                {
                    case TokenType.OpenParenthesis:
                    case TokenType.OpenBracket:
                    case TokenType.OpenBrace:
                        tail = ParseStatementTail(stream);
                        break;
                    case TokenType.GreaterThan:
                        //consume greater than
                        stream.NextNoReturn();

                        //might be a single child or a statement list of siblings
                        tail = ParseSingleStatementTail(stream);
                        break;
                        //case TokenType.Colon:
                        //    //next thing must be an identifier
                        //    var colon = stream.Next();
                        //    identifier = stream.Next();
                        //    statement = new EncodedOutput(identifier.Content);
                        //    goto checkForSiblings;
                        //case TokenType.Equal:
                        //    //next thing must be an identifier
                        //    var equal = stream.Next();
                        //    identifier = stream.Next();
                        //    statement = new RawOutput(identifier.Content);
                        //    goto checkForSiblings;

                    default:
                        GetStatementFromToken(identifier, tail);
                        goto checkForSiblings;
                }
            }


            checkForSiblings:
            statement = GetStatementFromToken(identifier, tail, previousToken);


            var list = new StatementList();
            list.Add(statement);

            while (stream.Peek() != null)
            {
                //Parrot.Debugger.Debug.WriteLine("Looking for siblings");
                if (stream.Peek().Type == TokenType.Plus)
                {
                    //it's now a statementlist not a statement
                    stream.NextNoReturn();
                    var siblings = ParseStatement(stream);
                    foreach (var sibling in siblings)
                    {
                        list.Add(sibling);
                    }
                }
                else
                {
                    break;
                }
            }

            return list;
        }

        private Statement GetStatementFromToken(Token identifier, StatementTail tail, Token previousToken = null)
        {
            var value = identifier != null ? identifier.Content : "";
            if (identifier != null)
            {
                switch (identifier.Type)
                {
                    case TokenType.StringLiteral:
                    case TokenType.QuotedStringLiteral:
                        return new StringLiteral(value, tail) {Index = identifier.Index};

                    case TokenType.StringLiteralPipe:
                        return new StringLiteralPipe(value.Substring(1), tail) {Index = identifier.Index};
                }
            }

            if (previousToken != null)
            {
                switch (previousToken.Type)
                {
                    case TokenType.At:
                        return new EncodedOutput(value) {Index = previousToken.Index};
                    case TokenType.Equal:
                        return new RawOutput(value) {Index = previousToken.Index};
                }
            }

            return new Statement(value, tail) {Index = identifier.Index};
        }

        private StatementTail ParseSingleStatementTail(Stream stream)
        {
            var statementList = ParseStatement(stream);
            //Parrot.Debugger.Debug.WriteLine("Found Single statement tail");
            return new StatementTail {Children = statementList};
        }

        private StatementTail ParseStatementTail(Stream stream)
        {
            //Parrot.Debugger.Debug.WriteLine("Parsing: StatementTail");

            //expect an attribute
            //expect a parameter
            //expect a list of children

            var additional = new object[3];

            while (stream.Peek() != null)
            {
                var token = stream.Peek();
                if (token == null)
                {
                    break;
                }

                switch (token.Type)
                {
                    case TokenType.OpenParenthesis:
                        additional[1] = ParseParameters(stream);
                        break;
                    case TokenType.OpenBracket:
                        additional[0] = ParseAttributes(stream);
                        break;
                    case TokenType.GreaterThan:
                        additional[2] = ParseChild(stream);
                        break;
                    case TokenType.OpenBrace:
                        //parse children
                        additional[2] = ParseChildren(stream);
                        goto productionFound;
                    default:
                        //no invalid token here
                        goto productionFound;
                }
            }

            productionFound:
            return new StatementTail
                {
                    Attributes = additional[0] as AttributeList,
                    Parameters = additional[1] as ParameterList,
                    Children = additional[2] as StatementList
                };
        }

        private StatementList ParseChild(Stream stream)
        {
            var child = new StatementList();

            var open = stream.Next();

            while (stream.Peek() != null)
            {
                var token = stream.Peek();
                if (token == null)
                {
                    break;
                }

                switch (token.Type)
                {
                    default:
                        var statements = ParseStatement(stream);
                        foreach (var statement in statements)
                        {
                            child.Add(statement);
                        }
                        goto doneWithChildren;
                }
            }

            doneWithChildren:
            return child;
        }

        private StatementList ParseChildren(Stream stream)
        {
            StatementList statement = new StatementList();
            var open = stream.Next();

            while (stream.Peek() != null)
            {
                var token = stream.Peek();
                if (token == null)
                {
                    break;
                }

                switch (token.Type)
                {
                    case TokenType.Plus:
                        //ignore these?
                        break;
                    case TokenType.CloseBrace:
                        //consume closing brace
                        stream.NextNoReturn();
                        goto doneWithChildren;
                    default:
                        var statements = ParseStatement(stream);
                        int length = statements.Count;
                        for (int i = 0; i < length; i++)
                        {
                            statement.Add(statements[i]);
                        }
                        break;
                }
            }

            doneWithChildren:
            return statement;
        }

        private ParameterList ParseParameters(Stream stream)
        {
            var parameterList = new ParameterList();

            //( parameterlist )
            var open = stream.Next();

            while (stream.Peek() != null)
            {
                var token = stream.Peek();
                if (token == null)
                {
                    break;
                }

                switch (token.Type)
                {
                    case TokenType.Identifier:
                    case TokenType.QuotedStringLiteral:
                    case TokenType.StringLiteralPipe:
                        parameterList.Add(ParseParameter(stream));
                        break;
                    case TokenType.Comma:
                        //another parameter - consume this
                        stream.NextNoReturn();
                        break;
                    case TokenType.CloseParenthesis:
                        //consume close parenthesis
                        stream.NextNoReturn();
                        goto doneWithParameter;
                    default:
                        //read until )
                        Errors.Add(new UnexpectedToken(token));
                        return parameterList;
                        //throw new ParserException(token);
                }
            }

            doneWithParameter:
            return parameterList;
        }

        private Parameter ParseParameter(Stream stream)
        {
            var identifier = stream.Next();
            switch (identifier.Type)
            {
                case TokenType.StringLiteralPipe:
                case TokenType.QuotedStringLiteral:
                case TokenType.StringLiteral:
                case TokenType.Identifier:
                    break;
                default:
                    //invalid token
                    Errors.Add(new UnexpectedToken(identifier));
                    //throw new ParserException(identifier);
                    return null;
            }

            //reduction
            return new Parameter(identifier.Content);
        }

        private AttributeList ParseAttributes(Stream stream)
        {
            var attributes = new AttributeList();

            var open = stream.Next();
            Token token = null;

            //expecting identifier
            while (stream.Peek() != null)
            {
                token = stream.Peek();
                if (token == null)
                {
                    break;
                }

                switch (token.Type)
                {
                    case TokenType.Identifier:
                        attributes.Add(ParseAttribute(stream));
                        break;
                    case TokenType.CloseBracket:
                        //consume close bracket
                        stream.NextNoReturn();
                        goto doneWithAttribute;
                    default:
                        //invalid token
                        Errors.Add(new AttributeIdentifierMissing
                            {
                                Index = token.Index
                            });
                        //throw new ParserException(token);
                        return attributes;
                }
            }

            doneWithAttribute:
            if (attributes.Count == 0)
            {
                //must be empty attribute list
                Errors.Add(new AttributeListEmpty
                    {
                        Index = token.Index
                    });
                return attributes;
                //throw new ParserException(token);
            }

            //do reduction here
            return attributes;
        }

        private Attribute ParseAttribute(Stream stream)
        {
            var identifier = stream.Next();
            var equalsToken = stream.Peek() as EqualToken;
            if (equalsToken != null)
            {
                stream.NextNoReturn();
                var valueToken = stream.Peek();
                if (valueToken == null)
                {
                    //TODO: Errors.Add(stream.Next());
                    Errors.Add(new UnexpectedToken(identifier));
                    return new Attribute(identifier.Content, null);
                    //throw new ParserException(string.Format("Unexpected end of stream"));
                }

                if (valueToken.Type == TokenType.CloseBracket)
                {
                    //then it's an invalid declaration
                    Errors.Add(new AttributeValueMissing {Index = valueToken.Index});
                }

                Statement value = ParseStatement(stream).SingleOrDefault();
                //force this as an attribute type
                if (value == null)
                {
                }
                else
                {
                    switch (value.Name)
                    {
                        case "true":
                        case "false":
                        case "null":
                            value = new StringLiteral("\"" + value.Name + "\"");
                            break;
                    }
                }

                //reduction
                return new Attribute(identifier.Content, value);
            }

            //single attribute only
            return new Attribute(identifier.Content, null);
        }
    }
}