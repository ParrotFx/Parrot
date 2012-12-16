// -----------------------------------------------------------------------
// <copyright file="Parser.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------


using System;
using System.IO;
using Parrot.Parser.ErrorTypes;

namespace Parrot.Parser
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Diagnostics;

    using Infrastructure;
    using Lexer;
    using Nodes;

    public class Parser
    {
        private readonly IHost _host;
        public IList<ParserError> Errors { get; private set; }

        public Parser(IHost host)
        {
            _host = host;
            Errors = new List<ParserError>();
        }

        public bool Parse(System.IO.Stream stream, out Document document)
        {
            document = new Document(_host);

            try
            {
                var tokenizer = new Tokenizer(stream);

                var tokens = tokenizer.Tokens();

                var tokenStream = new Stream(tokens);

                foreach (var statement in Parse(tokenStream))
                {
                    foreach (var s in statement)
                    {
                        if (s.Errors.Any())
                        {
                            foreach (var error in s.Errors)
                            {
                                error.Index += s.Index;
                                Errors.Add(error);
                            }
                        }

                        document.Children.Add(s);
                    }
                }
            }
            catch (EndOfStreamException)
            {
                Errors.Add(new EndOfStream() { Index = (int)stream.Length });
            }

            document.Errors = Errors;

            return true;
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
                    return new StatementList(_host);
                //throw new ParserException(stream.Peek());
            }


            Statement statement = null;
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
                    //    statement = new EncodedOutput(_host, identifier.Content);
                    //    goto checkForSiblings;
                    //case TokenType.Equal:
                    //    //next thing must be an identifier
                    //    var equal = stream.Next();
                    //    identifier = stream.Next();
                    //    statement = new RawOutput(_host, identifier.Content);
                    //    goto checkForSiblings;

                    default:
                        statement = GetStatementFromToken(identifier, tail);
                        goto checkForSiblings;
                }
            }


        checkForSiblings:
            statement = GetStatementFromToken(identifier, tail, previousToken);


            var list = new StatementList(_host);
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
                        return new StringLiteral(_host, value, tail) { Index = identifier.Index };

                    case TokenType.StringLiteralPipe:
                        return new StringLiteralPipe(_host, value.Substring(1), tail) { Index = identifier.Index };
                }
            }

            if (previousToken != null)
            {
                switch (previousToken.Type)
                {
                    case TokenType.At:
                        return new EncodedOutput(_host, value) { Index = previousToken.Index };
                    case TokenType.Equal:
                        return new RawOutput(_host, value) { Index = previousToken.Index };
                }
            }

            return new Statement(_host, value, tail) { Index = identifier.Index };
        }

        private StatementTail ParseSingleStatementTail(Stream stream)
        {
            var statementList = ParseStatement(stream);
            //Parrot.Debugger.Debug.WriteLine("Found Single statement tail");
            return new StatementTail(_host) { Children = statementList };
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
            return new StatementTail(_host)
            {
                Attributes = additional[0] as AttributeList,
                Parameters = additional[1] as ParameterList,
                Children = additional[2] as StatementList
            };
        }

        private StatementList ParseChild(Stream stream)
        {
            var child = new StatementList(_host);

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
            StatementList statement = new StatementList(_host);
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
            var parameterList = new ParameterList(_host);

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
            return new Parameter(_host, identifier.Content);
        }

        private AttributeList ParseAttributes(Stream stream)
        {
            var attributes = new AttributeList(_host);

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
                        Errors.Add(new AttributeIdentifierMissing()
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
                Errors.Add(new AttributeListEmpty()
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
                Statement value;
                var valueToken = stream.Peek();
                if (valueToken == null)
                {
                    //TODO: Errors.Add(stream.Next());
                    Errors.Add(new UnexpectedToken(identifier));
                    return new Attribute(_host, identifier.Content, null);
                    //throw new ParserException(string.Format("Unexpected end of stream"));
                }

                if (valueToken.Type == TokenType.CloseBracket)
                {
                    //then it's an invalid declaration
                    Errors.Add(new AttributeValueMissing() { Index = valueToken.Index });
                }

                value = ParseStatement(stream).SingleOrDefault();
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
                            value = new StringLiteral(_host, "\"" + value.Name + "\"");
                            break;
                    }
                }

                //reduction
                return new Attribute(_host, identifier.Content, value);
            }

            //single attribute only
            return new Attribute(_host, identifier.Content, null);
        }

    }
}
