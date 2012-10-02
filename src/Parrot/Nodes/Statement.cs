using System;
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

        internal Statement(IHost host) : base(host)
        {
            Attributes = new AttributeList(host);
            Children = new StatementList(host);
            Parameters = new ParameterList(host);
        }

        protected Statement(IHost host, string name)
            : this(host)
        {

            //required bullshit
            if (name.IndexOfAny(new [] { '.', '#', ':'}) > -1)
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

                            AddAttribute(new Attribute(Host, "id", new StringLiteral(host, "\"" + part.Name + "\"")));
                            break;
                        case IdentifierType.Class:
                            if (part.Name.Length == 1)
                            {
                                throw new ParserException("Id must have a length");
                            }

                            AddAttribute(new Attribute(Host, "class", new StringLiteral(host, "\"" + part.Name + "\"")));
                            break;

                        case IdentifierType.Type:
                            AddAttribute(new Attribute(host, "type", new StringLiteral(host, "\"" + part.Name + "\"")));
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
            ParseStatementTail(statementTail);
        }

        protected internal void ParseStatementTail(StatementTail statementTail)
        {
            if (statementTail != null)
            {
                if (statementTail.Parameters != null)
                {
                    Parameters = statementTail.Parameters;
                }

                //AddAttributes(statementTail.Attributes);
                if (statementTail.Attributes != null)
                {
                    for (int i = 0; i < Attributes.Count; i++)
                    {
                        statementTail.Attributes.Add(Attributes[i]);
                    }
                    Attributes = statementTail.Attributes;
                }

                if (statementTail.Children != null)
                {
                    Children = statementTail.Children;
                }
            }
        }

        private void AddAttributes(AttributeList attributes)
        {
            if (attributes == null) return;

            int length = attributes.Count;
            for (int i = 0; i < length; i++)
            {
                Attributes.Add(attributes[i]);
            }
        }

        private void AddAttribute(Attribute node)
        {
            //if (node.Key == "id")
            //{
            //    var nodeValue = node.Value as string;

            //    if (nodeValue != null && nodeValue.Contains("."))
            //    {
            //        var values = nodeValue.Split(".".ToCharArray());
            //        foreach (var value in values.Skip(1))
            //        {
            //            Attributes.Add(new Attribute(Host, "class", value));
            //        }

            //        Attributes.Add(new Attribute(Host, node.Key, values[0]));
            //        return;
            //    }
            //}

            Attributes.Add(node);
        }

        public override bool IsTerminal
        {
            get { return false; }
        }

        private void IdentifierTypeFromCharacter(char character, ref IdentifierType currentType)
        {
            switch (character)
            {
                case ':': 
                    currentType = IdentifierType.Type;
                    break;
                case '#': 
                    currentType = IdentifierType.Id;
                    break;
                case '.': 
                    currentType = IdentifierType.Class;
                    break;
            }
        }

        private IEnumerable<Identifier> GetIdentifierParts(string source)
        {
            int index = 0;
            int partLocation = 0;

            var partType = IdentifierType.Literal;
            IdentifierType nextType = IdentifierType.None;

            for (int i = 0; i < source.Length; i++)
            {
                IdentifierTypeFromCharacter(source[i], ref nextType);

                if (nextType != IdentifierType.None)
                {
                    yield return new Identifier
                    {
                        Name = source.Substring(index, i - index),
                        Type = partType
                    };

                    index = i + 1;
                    partType = nextType;
                    nextType = IdentifierType.None;
                }

            }

            yield return new Identifier
            {
                Name = source.Substring(index),
                Type = partType
            };
        }
    }
}