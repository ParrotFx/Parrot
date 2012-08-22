namespace Parrot.Parser
{
    using System.Reflection;
    using System.IO;
    using GOLD;
    using Nodes;

    public class Parser
    {

        private GOLD.Parser _parser = new GOLD.Parser();

        private enum ProductionIndex
        {
            // ReSharper disable InconsistentNaming
            @Parameter_Stringliteral = 0,              // <Parameter> ::= StringLiteral
            @Parameter_Identifier = 1,                 // <Parameter> ::= Identifier
            @Parameterlist = 2,                        // <Parameter List> ::= <Parameter>
            @Parameterlist_Comma = 3,                  // <Parameter List> ::= <Parameter List> ',' <Parameter>
            @Parameters_Lparan_Rparan = 4,             // <Parameters> ::= '(' <Parameter List> ')'
            @Parameters = 5,                           // <Parameters> ::= 
            @Attribute_Identifier_Eq_Stringliteral = 6,  // <Attribute> ::= Identifier '=' StringLiteral
            @Attribute_Identifier_Eq_Identifier = 7,   // <Attribute> ::= Identifier '=' Identifier
            @Attribute_Identifier = 8,                 // <Attribute> ::= Identifier
            @Attributelist = 9,                        // <Attribute List> ::= <Attribute>
            @Attributelist2 = 10,                      // <Attribute List> ::= <Attribute List> <Attribute>
            @Attributes_Lbracket_Rbracket = 11,        // <Attributes> ::= '[' <Attribute List> ']'
            @Attributes = 12,                          // <Attributes> ::= 
            @Sibling_Plus = 13,                        // <Sibling> ::= <Statement> '+' <Statement>
            @Sibling_Plus2 = 14,                       // <Sibling> ::= <Sibling> '+' <Statement>
            @Parentchild_Gt = 15,                      // <ParentChild> ::= <Statement> '>' <Statement>
            @Child = 16,                               // <Child> ::= <ParentChild>
            @Child_Gt = 17,                            // <Child> ::= <Statement> '>' <Sibling>
            @Child_Gt2 = 18,                           // <Child> ::= <Statement> '>' <Child>
            @Statements = 19,                          // <Statements> ::= <Statement>
            @Statements2 = 20,                         // <Statements> ::= <Statements> <Statement>
            @Statements3 = 21,                         // <Statements> ::= <Sibling>
            @Statements4 = 22,                         // <Statements> ::= <Child>
            @Statements5 = 23,                         // <Statements> ::= <Statements> <Child>
            @Statementtail_Lbrace_Rbrace = 24,         // <Statement Tail> ::= <Attributes> <Parameters> '{' <Statements> '}'
            @Statementtail_Lbrace_Rbrace2 = 25,        // <Statement Tail> ::= <Attributes> <Parameters> '{' '}'
            @Statementtail = 26,                       // <Statement Tail> ::= <Attributes> <Parameters>
            @Statement_Identifier = 27,                // <Statement> ::= Identifier <Statement Tail>
            @Statement = 28,                           // <Statement> ::= <OutputStatement>
            @Statement_Multilinestringliteral = 29,    // <Statement> ::= MultiLineStringLiteral
            @Statement_Stringliteral = 30,             // <Statement> ::= StringLiteral
            @Statement_Stringliteralpipe = 31,         // <Statement> ::= StringLiteralPipe
            @Outputstatement_Colon_Identifier = 32,    // <OutputStatement> ::= ':' Identifier
            @Outputstatement_Eq_Identifier = 33        // <OutputStatement> ::= '=' Identifier
            // ReSharper restore InconsistentNaming
        }

        static Parser()
        {
            //This procedure can be called to load the parse tables. The class can
            //read tables using a BinaryReader.
            System.AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                Assembly assembly = Assembly.GetExecutingAssembly();

                string name = args.Name.Substring(0, args.Name.IndexOf(','));

                Stream stream = assembly.GetManifestResourceStream(string.Format("Parrot.{0}.dll", name));
                byte[] block = new byte[stream.Length];
                stream.Read(block, 0, block.Length);
                Assembly a2 = Assembly.Load(block);
                return a2;
            };

            ParserFactory.InitializeFactoryFromResource("Parrot.parrot.egt");
        }

        public bool Parse(TextReader reader, out Document document)
        {
            document = null;

            _parser = ParserFactory.CreateParser(reader);

            //This procedure starts the GOLD Parser Engine and handles each of the
            //messages it returns. Each time a reduction is made, you can create new
            //custom object and reassign the .CurrentReduction property. Otherwise, 
            //the system will use the Reduction object that was returned.
            //
            //The resulting tree will be a pure representation of the language 
            //and will be ready to implement.

            ParseMessage message;
            bool done;                      //Controls when we leave the loop
            bool accepted = false;          //Was the parse successful?

            _parser.Open(reader);
            _parser.TrimReductions = false;  //Please read about this feature before enabling  

            done = false;
            while (!done)
            {
                var response = _parser.Parse();

                switch (response)
                {
                    case ParseMessage.LexicalError:
                        //Cannot recognize token
                        throw new Parrot.ParserException("find the message somehow");

                    case ParseMessage.SyntaxError:
                        //Expecting a different token
                        throw new Parrot.ParserException("find the message somehow");

                    case ParseMessage.Reduction:
                        //Create a customized object to store the reduction

                        _parser.CurrentReduction = CreateNewObject((Reduction)_parser.CurrentReduction);
                        break;

                    case ParseMessage.Accept:
                        //Accepted!
                        document = _parser.CurrentReduction as Document;
                        //program = parser.CurrentReduction   //The root node!                 
                        done = true;
                        accepted = true;
                        break;

                    case ParseMessage.TokenRead:
                        //You don't have to do anything here.
                        break;

                    case ParseMessage.InternalError:
                        //INTERNAL ERROR! Something is horribly wrong.
                        done = true;
                        break;

                    case ParseMessage.NotLoadedError:
                        //This error occurs if the CGT was not loaded.                   
                        done = true;
                        break;

                    case ParseMessage.GroupError:
                        //GROUP ERROR! Unexpected end of file
                        done = true;
                        break;
                }
            } //while

            return accepted;
        }

        private object CreateNewObject(Reduction r)
        {
            object result = null;

            switch ((ProductionIndex)r.Parent.TableIndex())
            {
                case ProductionIndex.Parameter_Stringliteral:
                    // <Parameter> ::= StringLiteral
                    return new Parameter(r[0].Data as string);

                case ProductionIndex.Parameter_Identifier:
                    // <Parameter> ::= Identifier
                    return new Parameter(r[0].Data as string);

                case ProductionIndex.Parameterlist:
                    // <Parameter List> ::= <Parameter>
                    return CreateParameterListFromParameter(r);

                case ProductionIndex.Parameters_Lparan_Rparan:
                    // <Parameters> ::= '(' <Parameter List> ')'
                    return r[1].Data as ParameterList;

                case ProductionIndex.Parameters:
                    // <Parameters> ::= 
                    return null;

                case ProductionIndex.Attribute_Identifier_Eq_Stringliteral:
                    // <Attribute> ::= Identifier '=' StringLiteral
                    return CreateAttributeNodeFromIdentifierStringLiteral(r);

                case ProductionIndex.Attribute_Identifier_Eq_Identifier:
                    // <Attribute> ::= Identifier '=' Identifier
                    return CreateAttributeNodeFromIdentifierIdentifier(r);

                case ProductionIndex.Attribute_Identifier:
                    // <Attribute> ::= Identifier
                    return new Attribute(r[0].Data as string, null);

                case ProductionIndex.Attributelist:
                    // <Attribute List> ::= <Attribute>
                    return CreateAttributeListFromAttribute(r);

                case ProductionIndex.Attributelist2:
                    // <Attribute List> ::= <Attribute List> <Attribute>
                    return new AttributeList(r[0].Data as AttributeList, r[1].Data as Attribute);

                case ProductionIndex.Attributes_Lbracket_Rbracket:
                    // <Attributes> ::= '[' <Attribute List> ']'
                    return r[1].Data as AttributeList;

                case ProductionIndex.Attributes:
                    // <Attributes> ::= 
                    return null;

                case ProductionIndex.Statements:
                    // <Statements> ::= <Statement>
                    return CreateDocumentNode(r);

                case ProductionIndex.Statements2:
                    // <Statements> ::= <Statements> <Statement>
                    return CreateBlockNodeList(r);

                case ProductionIndex.Statements3:
                    return CreateDocumentNode(r);

                case ProductionIndex.Statements4:
                    return CreateDocumentNode(r);
                case ProductionIndex.@Statements5:
                    // <Statements> ::= <Statements> <Child>
                    var tempdoc = r[0].Data as Document;
                    var tempchild = r[1].Data as Statement;
                    tempdoc.Children.Add(tempchild);

                    return tempdoc;

                case ProductionIndex.Sibling_Plus:

                    return new StatementList(r[0].Data as Statement, r[2].Data as Statement);

                case ProductionIndex.Sibling_Plus2:
                    return new StatementList(r[0].Data as StatementList, r[2].Data as Statement);

                case ProductionIndex.Statementtail:
                    // <Statement Tail> ::= <Attributes> <Parameters>
                    return CreateStatementTailWithAttributesParameters(r);

                case ProductionIndex.Statementtail_Lbrace_Rbrace2:
                    // <Statement Tail> ::= <Attributes> <Parameters> '{' '}'
                    return new StatementTail
                    {
                        Attributes = r[0].Data as AttributeList,
                        Parameters = r[1].Data as ParameterList
                    };

                case ProductionIndex.Statementtail_Lbrace_Rbrace:
                    // <Statement Tail> ::= <Attributes> <Parameters> '{' <Statements> '}'
                    return new StatementTail
                    {
                        Attributes = r[0].Data as AttributeList,
                        Parameters = r[1].Data as ParameterList,
                        Children = (r[3].Data as Document).Children
                    };

                case ProductionIndex.@Child:
                    // <Child> ::= <ParentChild>
                    return r[0].Data as Statement;

                case ProductionIndex.Child_Gt:
                    // <Child> ::= <Statement> '>' <Sibling>

                    var parent = r[0].Data as Statement;
                    var children = r[2].Data as StatementList;

                    foreach (var child in children)
                    {
                        parent.Children.Add(child);
                    }

                    return parent;

                case ProductionIndex.Child_Gt2:
                    //parent then statementlist
                    //<Statement> '>' <Child>
                    var parent2 = r[0].Data as Statement;
                    var child2 = r[2].Data as Statement;
                    parent2.Children.Add(child2);

                    return parent2;

                case ProductionIndex.Parentchild_Gt:
                    // <ParentChild> ::= <Statement> '>' <Statement>:
                    var parent3 = r[0].Data as Statement;
                    var child1 = r[2].Data as Statement;
                    parent3.Children.Add(child1);

                    return parent3;

                case ProductionIndex.Statement:
                    // <Statement> ::= <OutputStatement>
                    return r[0].Data;

                case ProductionIndex.Statement_Identifier:
                    // <Statement> ::= Identifier <Statement Tail>
                    return CreateStatementIdentifier(r);

                case ProductionIndex.Statement_Stringliteralpipe:
                    // <Statement> ::= StringLiteralPipe
                    return new StringLiteralPipe((r[0].Data as string).Substring(1));

                case ProductionIndex.Statement_Multilinestringliteral:
                case ProductionIndex.Statement_Stringliteral:
                    // <Statement> ::= MultiLineStringLiteral
                    // <Statement> ::= StringLiteral
                    return new StringLiteral(r[0].Data as string);

                case ProductionIndex.Outputstatement_Colon_Identifier:
                    // <OutputStatement> ::= ':' Identifier
                    return new EncodedOutput(r[1].Data as string);

                case ProductionIndex.Outputstatement_Eq_Identifier:
                    // <OutputStatement> ::= '=' Identifier
                    return new RawOutput(r[1].Data as string);

            }  //switch

            return result;
        }

        private Attribute CreateAttributeNodeFromIdentifierIdentifier(Reduction reduction)
        {
            return new Attribute(reduction[0].Data as string, reduction[2].Data as string);
        }

        private Document CreateBlockNodeList(Reduction reduction)
        {
            // <Statements> ::= <Statements> <Statement>
            return new Document(reduction[0].Data as Document, reduction[1].Data as Statement);
        }

        private StatementTail CreateStatementTailWithAttributesParameters(Reduction reduction)
        {
            return new StatementTail
            {
                Attributes = reduction[0].Data as AttributeList,
                Parameters = reduction[1].Data as ParameterList
            };
        }

        private Attribute CreateAttributeNodeFromIdentifierStringLiteral(Reduction reduction)
        {
            return new Attribute(reduction[0].Data as string, reduction[2].Data as string);
        }

        private ParameterList CreateParameterListFromParameter(Reduction reduction)
        {
            return new ParameterList(reduction[0].Data as Parameter);
        }

        private AttributeList CreateAttributeListFromAttribute(Reduction reduction)
        {
            return new AttributeList(reduction[0].Data as Attribute);
        }

        private Document CreateDocumentNode(Reduction reduction)
        {
            return new Document()
            {
                Children = reduction[0].Data as StatementList ?? new StatementList(reduction[0].Data as Statement)
            };
        }

        private Statement CreateStatementIdentifier(Reduction reduction)
        {
            // <Statement> ::= Identifier <Statement Tail>
            return new Statement(reduction[0].Data as string, reduction[1].Data as StatementTail);
        }
    } //MyParser
}