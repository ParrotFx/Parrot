namespace Parrot.Nodes
{
    using System.Collections.Generic;
    using System.Linq;
    using Parrot.Parser.ErrorTypes;

    public class Statement : AbstractNode
    {
        public ParameterList Parameters { get; private set; }
        public string Name { get; set; }
        public AttributeList Attributes { get; private set; }
        public StatementList Children { get; private set; }
        public int Index { get; set; }
        public int Length { get; set; }
        public IList<Identifier> IdentifierParts { get; private set; }
        internal List<ParserError> Errors { get; set; }

        internal Statement()
        {
            Attributes = new AttributeList();
            Children = new StatementList();
            Parameters = new ParameterList();
            IdentifierParts = new List<Identifier>();
            Errors = new List<ParserError>();
        }

        protected Statement(string name) : this()
        {
            //required bullshit
            if (name.IndexOfAny(new[] {'.', '#', ':'}) > -1)
            {
                foreach (var part in GetIdentifierParts(name))
                {
                    switch (part.Type)
                    {
                        case IdentifierType.Id:
                            if (part.Name.Length == 0)
                            {
                                Errors.Add(new MissingIdDeclaration {Index = part.Index - 1, Length = 1});
                                //throw new ParserException("Id must have a length");
                            }
                            else if (Attributes.Any(a => a.Key == "id"))
                            {
                                Errors.Add(new MultipleIdDeclarations(part.Name) {Index = part.Index - 1, Length = part.Name.Length + 1});
                                //throw new ParserException("Id added more than once");
                            }
                            else
                            {
                                AddAttribute(new Attribute("id", new StringLiteral("\"" + part.Name + "\"")));
                            }
                            break;
                        case IdentifierType.Class:
                            if (part.Name.Length == 0)
                            {
                                Errors.Add(new MissingClassDeclaration {Index = part.Index - 1, Length = 1});
                                //throw new ParserException("Id must have a length");
                            }
                            else
                            {
                                AddAttribute(new Attribute("class", new StringLiteral("\"" + part.Name + "\"")));
                            }
                            break;

                        case IdentifierType.Type:
                            AddAttribute(new Attribute("type", new StringLiteral("\"" + part.Name + "\"")));
                            break;

                        case IdentifierType.Literal:
                            Name = part.Name;
                            break;
                    }

                    IdentifierParts.Add(part);
                }
            }
            else
            {
                Name = name;
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public Statement(string name, StatementTail statementTail) : this(name)
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
                    foreach (Attribute attribute in Attributes)
                    {
                        statementTail.Attributes.Add(attribute);
                    }
                    Attributes = statementTail.Attributes;
                }

                if (statementTail.Children != null)
                {
                    Children = statementTail.Children;
                }
            }
        }

        private void AddAttribute(Attribute node)
        {
            Attributes.Add(node);
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
                            Type = partType,
                            Index = index,
                            Length = i - index,
                        };

                    index = i + 1;
                    partType = nextType;
                    nextType = IdentifierType.None;
                }
            }

            yield return new Identifier
                {
                    Name = source.Substring(index),
                    Type = partType,
                    Index = index
                };
        }
    }
}