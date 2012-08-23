using Parrot.Infrastructure;

namespace Parrot.Nodes
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Parrot;

    public class Statement : AbstractNode
    {
        public ParameterList Parameters { get; private set; }
        public string Name { get; set; }
        public AttributeList Attributes { get; private set; }
        public StatementList Children { get; private set; }

        protected Statement(IHost host, string name) : base(host)
        {
            Attributes = new AttributeList(host);
            Children = new StatementList(host);
            Parameters = new ParameterList(host);

            //required bullshit
            if (name.Contains(".") || name.Contains("#") || name.Contains(":"))
            {
                foreach (var part in GetIdentifierParts(name))
                {
                    switch (part.Type)
                    {
                        case IdentifierType.Id:
                            if (part.Name.Length == 1)
                            {
                                throw new ParserException("Id must have a length");
                            }

                            if (Attributes.Any(a => a.Key == "id"))
                            {
                                throw new ParserException("Id added more than once");
                            }

                            AddAttribute(new Attribute(Host, "id", "\"" + part.Name + "\""));
                            break;
                        case IdentifierType.Class:
                            if (part.Name.Length == 1)
                            {
                                throw new ParserException("Id must have a length");
                            }

                            AddAttribute(new Attribute(Host, "class", "\"" + part.Name + "\""));
                            break;

                        case IdentifierType.Type:
                            AddAttribute(new Attribute(host, "type", "\"" + part.Name + "\""));
                            break;

                        case IdentifierType.Literal:
                            Name = part.Name;
                            break;
                    }
                }
            }
            else
            {
                Name = name;
            }

        }

        public Statement(IHost host, string name, StatementTail statementTail) : this(host, name)
        {
            if (statementTail != null)
            {
                AddParameters(statementTail.Parameters);
                AddAttributes(statementTail.Attributes);
                AddChildren(statementTail.Children);
            }
        }

        private void AddAttributes(AttributeList attributes)
        {
            if (attributes != null && attributes.Any())
            {
                foreach (var attribute in attributes)
                {
                    Attributes.Add(attribute);
                }
            }
        }

        private void AddParameters(ParameterList parameters)
        {
            if (parameters != null && parameters.Any())
            {
                foreach (var parameter in parameters)
                {
                    Parameters.Add(parameter);
                }
            }
        }

        private void AddChildren(StatementList statements)
        {
            if (statements != null && statements.Any())
            {
                foreach (var statement in statements)
                {
                    Children.Add(statement);
                }
            }
        }

        private void AddAttribute(Attribute node)
        {
            if (node.Key == "id")
            {
                var nodeValue = node.Value as string;

                if (nodeValue != null && nodeValue.Contains("."))
                {
                    var values = nodeValue.Split(".".ToCharArray());
                    foreach (var value in values.Skip(1))
                    {
                        Attributes.Add(new Attribute(Host, "class", value));
                    }

                    Attributes.Add(new Attribute(Host, node.Key, values[0]));
                    return;
                }
            }

            Attributes.Add(node);
        }

        public override bool IsTerminal
        {
            get { return false; }
        }

        //TODO: Refactor this - too much duplicated code
        private IEnumerable<Identifier> GetIdentifierParts(string source)
        {
            char[] splitBy = new[] { '.', '#', ':' };
            int start = 0;
            int index;

            string previousCharacter = "";

            //check for a starting character
            index = source.IndexOfAny(splitBy, start);
            if (index == 0)
            {
                previousCharacter = source.Substring(0, 1);
                source = source.Substring(1);
            }

            while ((index = source.IndexOfAny(splitBy, start)) != -1)
            {
                index++;
                index = Interlocked.Exchange(ref start, index);

                switch (previousCharacter)
                {
                    case "#":
                        yield return new Identifier
                        {
                            Name = source.Substring(index, start - index - 1),
                            Type = IdentifierType.Id
                        };
                        break;
                    case ".":
                        yield return new Identifier
                        {
                            Name = source.Substring(index, start - index - 1),
                            Type = IdentifierType.Class
                        };
                        break;
                    case ":":
                        yield return new Identifier
                        {
                            Name = source.Substring(index, start - index - 1),
                            Type = IdentifierType.Type
                        };
                        break;
                    default:
                        yield return new Identifier
                        {
                            Name = source.Substring(index, start - index - 1),
                            Type = IdentifierType.Literal
                        };
                        break;
                }

                previousCharacter = source.Substring(start - 1, 1);
            }

            if (start < source.Length)
            {
                switch (previousCharacter)
                {
                    case "#":
                        yield return new Identifier
                        {
                            Name = source.Substring(start),
                            Type = IdentifierType.Id
                        };
                        break;
                    case ".":
                        yield return new Identifier
                        {
                            Name = source.Substring(start),
                            Type = IdentifierType.Class
                        };
                        break;
                    case ":":
                        yield return new Identifier
                        {
                            Name = source.Substring(start),
                            Type = IdentifierType.Type
                        };
                        break;
                    default:
                        yield return new Identifier
                        {
                            Name = source.Substring(start),
                            Type = IdentifierType.Literal
                        };
                        break;
                }
            }
        }
    }
}