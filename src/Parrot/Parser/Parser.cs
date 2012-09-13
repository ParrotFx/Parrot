// -----------------------------------------------------------------------
// <copyright file="Parser.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics;
using Parrot.Infrastructure;
using Parrot.Lexer;
using Parrot.Nodes;

namespace Parrot.Parser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class Parser
    {
        //public Func<Production, object[], object> ProductionFound;
        private IHost _host;

        public Parser(IHost host)
        {
            _host = host;
        }

        public bool Parse(string text, out Document document)
        {
            var tokenizer = new Tokenizer(text);

            var tokens = tokenizer.Tokens();

            var stream = new Stream<Token>(tokens);

            document = new Document(_host);

            foreach (var statement in Parse(stream))
            {
                if (statement is Statement)
                {
                    document.Children.Add(statement as Statement);
                }
                else if (statement is StatementList)
                {
                    foreach (var s in (statement as StatementList))
                    {
                        document.Children.Add(s);
                    }
                }
            }

            return true;
        }

        private IEnumerable<object> Parse(Stream<Token> stream)
        {
            Stopwatch watch = Stopwatch.StartNew();

            List<Statement> siblings = new List<Statement>();

            while (stream.Peek() != null)
            {
                var token = stream.Peek();
                switch (token.Type)
                {
                    case TokenType.StringLiteral:
                    case TokenType.MultiLineStringLiteral:
                    case TokenType.StringLiteralPipe:
                    case TokenType.QuotedStringLiteral:
                    case TokenType.Identifier:
                    case TokenType.OpenBracket:
                    case TokenType.OpenParenthesis:
                        TokenFound(token);
                        var statement = ParseStatement(stream);

                        //if (siblings.Any())
                        //{
                        //    //if we got here then we aren't joining siblings anymore
                        //    yield return CreateStatementListFromSiblingList(siblings);
                        //    siblings = new List<Statement>();
                        //}

                        //if (stream.Peek() != null && stream.Peek().Type == TokenType.Plus)
                        //{
                        //    siblings.Add(statement);

                        //    //add to siblinglist
                        //}
                        //else
                        //{
                        yield return statement;
                        //}

                        break;
                    //case TokenType.Plus:
                    //    TokenFound(token);
                    //    siblings.Add(ParseSibling(stream));
                    //    break;
                    default:
                        throw new ParserException(token);
                }

            }

            watch.Stop();
            Parrot.Debugger.Debug.WriteLine(watch.Elapsed);

            //Accept
            if (siblings.Any())
            {
                yield return CreateStatementListFromSiblingList(siblings);
            }
        }

        private StatementList CreateStatementListFromSiblingList(List<Statement> productions)
        {
            ProductionFound(Production.StatementList);

            return new StatementList(_host, productions.ToArray());
        }

        //let is know a token is found
        private void TokenFound(Token token)
        {
            Parrot.Debugger.Debug.WriteLine("Token found: {0} ({1})", token.Type, token.Content);
        }

        //let us know a production was found
        private void ProductionFound(Production production)
        {
            Parrot.Debugger.Debug.WriteLine("Production found: {0}", production);
        }


        /// <summary>
        /// Parses a token stream creating the largest statement possible
        /// </summary>
        /// <param name="stream">Stream of tokens to parse</param>
        /// <returns>Statement</returns>
        private StatementList ParseStatement(Stream<Token> stream)
        {
            //statements can be made up of identifiers, attributes, parameters and children

            var tokenType = stream.Peek().Type;
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
                case TokenType.MultiLineStringLiteral:
                case TokenType.StringLiteralPipe:
                case TokenType.QuotedStringLiteral:
                    identifier = stream.Next();
                    break;
            }


            Statement statement = null;
            StatementTail tail = null;

            //3 items
            //attributes, parameters, children

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
                    case TokenType.OpenBrace:
                    case TokenType.OpenBracket:
                        tail = ParseStatementTail(stream);
                        break;
                    case TokenType.GreaterThan:
                        //consume greater than
                        stream.Next();
                        //might be a single child or a statement list of siblings
                        tail = ParseSingleStatementTail(stream);
                        break;
                    case TokenType.Colon:
                        //next thing must be an identifier
                        var colon = stream.Next();
                        identifier = stream.Next();
                        statement = new EncodedOutput(_host, identifier.Content);
                        goto checkForSiblings;
                    case TokenType.Equal:
                        //next thing must be an identifier
                        var equal = stream.Next();
                        identifier = stream.Next();
                        statement = new RawOutput(_host, identifier.Content);
                        goto checkForSiblings;

                    default:
                        statement = GetStatementFromToken(identifier, tail);
                        goto checkForSiblings;
                }
            }

            statement = GetStatementFromToken(identifier, tail);

        checkForSiblings:


            var list = new StatementList(_host, statement);

            while (stream.Peek() != null)
            {
                Parrot.Debugger.Debug.WriteLine("Looking for siblings");
                if (stream.Peek().Type == TokenType.Plus)
                {
                    //it's now a statementlist not a statement
                    stream.Next();
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

        private Statement GetStatementFromToken(Token identifier, StatementTail tail)
        {
            var value = identifier != null ? identifier.Content : "";
            if (identifier != null)
            {
                switch (identifier.Type)
                {
                    case TokenType.StringLiteral:
                        return new StringLiteral(_host, value);
                    case TokenType.QuotedStringLiteral:
                        return new StringLiteral(_host, value);
                    case TokenType.StringLiteralPipe:
                        return new StringLiteralPipe(_host, value.Substring(1));
                    case TokenType.MultiLineStringLiteral:
                        return new StringLiteral(_host, value);
                }
            }

            return new Statement(_host, value, tail);
        }

        private StatementTail ParseSingleStatementTail(Stream<Token> stream)
        {
            var statementList = ParseStatement(stream);
            Parrot.Debugger.Debug.WriteLine("Found Single statement tail");
            return new StatementTail(_host) { Children = statementList };
        }

        private StatementTail ParseStatementTail(Stream<Token> stream)
        {
            Parrot.Debugger.Debug.WriteLine("Parsing: StatementTail");

            //expect an attribute
            //expect a parameter
            //expect a list of children

            var additional = new object[3];

            while (stream.Peek() != null)
            {
                var token = stream.Peek();
                TokenFound(token);
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
                        additional[2] = ParseChildren(stream);
                        break;
                    case TokenType.OpenBrace:
                        //parse children
                        additional[2] = ParseChildren(stream);
                        goto productionFound;
                    //case TokenType.Identifier:
                    //    additional[2] = ParseChildren(stream);
                    //    goto productionFound;
                    default:
                        //no invalid token here
                        goto productionFound;
                }
            }

        productionFound:
            ProductionFound(Production.StatementTail);
            return new StatementTail(_host)
            {
                Attributes = additional[0] as AttributeList,
                Parameters = additional[1] as ParameterList,
                Children = additional[2] as StatementList
            };
        }

        private StatementList ParseChildren(Stream<Token> stream)
        {
            var open = stream.Next();
            CloseBracesToken close = null;
            List<Statement> children = new List<Statement>();

            while (stream.Peek() != null)
            {
                var token = stream.Peek();
                TokenFound(token);
                if (token == null)
                {
                    break;
                }

                switch (token.Type)
                {
                    case TokenType.Plus:
                        break;
                    //case TokenType.Identifier:
                    //    //we're done
                    //    goto doneWithChildren;
                    case TokenType.CloseBrace:
                        close = stream.Next() as CloseBracesToken;
                        goto doneWithChildren;
                    default:
                        var statements = ParseStatement(stream);
                        foreach (var statement in statements)
                        {
                            children.Add(statement);
                        }
                        break;
                }
            }

        doneWithChildren:
            ProductionFound(Production.ParameterGroup);
            return new StatementList(_host, children.ToArray());
        }

        private ParameterList ParseParameters(Stream<Token> stream)
        {
            //( parameterlist )
            var open = stream.Next();
            CloseParenthesisToken close = null;
            List<Parameter> children = new List<Parameter>();

            while (stream.Peek() != null)
            {
                var token = stream.Peek();
                TokenFound(token);
                if (token == null)
                {
                    break;
                }

                switch (token.Type)
                {
                    case TokenType.Identifier:
                    case TokenType.QuotedStringLiteral:
                    case TokenType.StringLiteralPipe:
                    case TokenType.MultiLineStringLiteral:
                        children.Add(ParseParameter(stream));
                        break;
                    case TokenType.CloseParenthesis:
                        close = stream.Next() as CloseParenthesisToken;
                        goto doneWithParameter;
                    default:
                        throw new ParserException(token);
                }
            }

        doneWithParameter:
            ProductionFound(Production.ParameterGroup);
            return new ParameterList(_host, children.ToArray());
        }

        private Parameter ParseParameter(Stream<Token> stream)
        {
            var identifier = stream.Next();
            switch (identifier.Type)
            {
                case TokenType.StringLiteralPipe:
                case TokenType.MultiLineStringLiteral:
                case TokenType.QuotedStringLiteral:
                case TokenType.StringLiteral:
                case TokenType.Identifier:
                    break;
                default:
                    //invalid token
                    throw new ParserException(identifier);
            }

            //reduction
            return new Parameter(_host, identifier.Content);
        }

        private AttributeList ParseAttributes(Stream<Token> stream)
        {
            var open = stream.Next();
            CloseBracketToken close = null;
            List<Parrot.Nodes.Attribute> children = new List<Parrot.Nodes.Attribute>();
            Token token  = null;

            //expecting identifier
            while (stream.Peek() != null)
            {
                token = stream.Peek();
                TokenFound(token);
                if (token == null)
                {
                    break;
                }

                switch (token.Type)
                {
                    case TokenType.Identifier:
                        children.Add(ParseAttribute(stream));
                        break;
                    case TokenType.CloseBracket:
                        close = stream.Next() as CloseBracketToken;
                        goto doneWithAttribute;
                    default:
                        //invalid token
                        throw new ParserException(token);
                }
            }

        doneWithAttribute:
            if (!children.Any())
            {
                //must be empty attribute list
                throw new ParserException(token);
            }
            //do reduction here
            ProductionFound(Production.AttributeGroup);
            return new AttributeList(_host, children.ToArray());
        }

        //expect identifier
        //expect equals
        //expect value

        private Parrot.Nodes.Attribute ParseAttribute(Stream<Token> stream)
        {
            var identifier = stream.Next();
            var equalsToken = stream.Peek() as EqualToken;
            if (equalsToken != null)
            {
                stream.Next();
                var valueToken = stream.Peek();
                if (valueToken == null)
                {
                    throw new ParserException(string.Format("Unexpected end of stream"));
                }

                stream.Next();

                switch (valueToken.Type)
                {
                    case TokenType.StringLiteralPipe:
                    case TokenType.MultiLineStringLiteral:
                    case TokenType.QuotedStringLiteral:
                    case TokenType.StringLiteral:
                    case TokenType.Identifier:
                        break;
                    default:
                        //invalid token
                        throw new ParserException(valueToken);
                }

                //reduction
                return new Parrot.Nodes.Attribute(_host, identifier.Content, valueToken.Content);
            }
            else
            {
                //single attribute only
                return new Parrot.Nodes.Attribute(_host, identifier.Content, null);
            }
        }

    }
}
