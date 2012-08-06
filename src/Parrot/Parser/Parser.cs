using System.IO;
using System.Text;
using GOLD;
using Parrot.Nodes;

namespace Parrot.Parser
{
    using System.Reflection;

    public class Parser
    {
        private GOLD.Parser _parser = new GOLD.Parser();

        private enum SymbolIndex
        {
            @Eof = 0,                                  // (EOF)
            @Error = 1,                                // (Error)
            @Comment = 2,                              // Comment
            @Newline = 3,                              // NewLine
            @Whitespace = 4,                           // Whitespace
            @Timesdiv = 5,                             // '*/'
            @Divtimes = 6,                             // '/*'
            @Divdiv = 7,                               // '//'
            @Lparan = 8,                               // '('
            @Rparan = 9,                               // ')'
            @Comma = 10,                               // ','
            @Colon = 11,                               // ':'
            @Semi = 12,                                // ';'
            @Lbracket = 13,                            // '['
            @Rbracket = 14,                            // ']'
            @Lbrace = 15,                              // '{'
            @Rbrace = 16,                              // '}'
            @Eq = 17,                                  // '='
            @Identifier = 18,                          // Identifier
            @Multilinestringliteral = 19,              // MultiLineStringLiteral
            @Stringliteral = 20,                       // StringLiteral
            @Stringliteralpipe = 21,                   // StringLiteralPipe
            @Attribute = 22,                           // <Attribute>
            @Attributelist = 23,                       // <Attribute List>
            @Attributes = 24,                          // <Attributes>
            @Outputstatement = 25,                     // <OutputStatement>
            @Parameter = 26,                           // <Parameter>
            @Parameterlist = 27,                       // <Parameter List>
            @Parameters = 28,                          // <Parameters>
            @Statement = 29,                           // <Statement>
            @Statementtail = 30,                       // <Statement Tail>
            @Statements = 31                           // <Statements>
        }

        private enum ProductionIndex
        {
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
            @Statements = 13,                          // <Statements> ::= <Statement>
            @Statements2 = 14,                         // <Statements> ::= <Statements> <Statement>
            @Statementtail_Lbrace_Rbrace = 15,         // <Statement Tail> ::= <Attributes> <Parameters> '{' <Statements> '}'
            @Statementtail_Lbrace_Rbrace2 = 16,        // <Statement Tail> ::= <Attributes> <Parameters> '{' '}'
            @Statementtail_Semi = 17,                  // <Statement Tail> ::= <Attributes> <Parameters> ';' <Statement>
            @Statementtail = 18,                       // <Statement Tail> ::= <Attributes> <Parameters>
            @Statement_Identifier = 19,                // <Statement> ::= Identifier <Statement Tail>
            @Statement = 20,                           // <Statement> ::= <OutputStatement>
            @Statement_Stringliteralpipe = 21,         // <Statement> ::= StringLiteralPipe
            @Statement_Multilinestringliteral = 22,    // <Statement> ::= MultiLineStringLiteral
            @Statement_Stringliteral = 23,             // <Statement> ::= StringLiteral
            @Outputstatement_Colon_Identifier = 24,    // <OutputStatement> ::= ':' Identifier
            @Outputstatement_Eq_Identifier = 25        // <OutputStatement> ::= '=' Identifier
        }

        private static BinaryReader GetResourceReader(string resourceName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream stream = assembly.GetManifestResourceStream(resourceName);
            return new BinaryReader(stream);
        }

        static Parser()
        {
            //This procedure can be called to load the parse tables. The class can
            //read tables using a BinaryReader.

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
                        throw new ParserException("find the message somehow");

                    case ParseMessage.SyntaxError:
                        //Expecting a different token
                        throw new ParserException("find the message somehow");

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

                case ProductionIndex.Statementtail_Semi:
                    // <Statement Tail> ::= <Attributes> <Parameters> ';' <Statement>
                    break;

                case ProductionIndex.Statement:
                    // <Statement> ::= <OutputStatement>
                    break;

                case ProductionIndex.Statement_Identifier:
                    // <Statement> ::= Identifier <Statement Tail>
                    return CreateStatementIdentifier(r);

                case ProductionIndex.Statement_Stringliteralpipe:
                    // <Statement> ::= StringLiteralPipe
                    break;

                case ProductionIndex.Statement_Multilinestringliteral:
                    // <Statement> ::= MultiLineStringLiteral
                    break;

                case ProductionIndex.Statement_Stringliteral:
                    // <Statement> ::= StringLiteral
                    break;

                case ProductionIndex.Outputstatement_Colon_Identifier:
                    // <OutputStatement> ::= ':' Identifier
                    break;

                case ProductionIndex.Outputstatement_Eq_Identifier:
                    // <OutputStatement> ::= '=' Identifier
                    break;

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

    public class StatementTail : AbstractNode
    {


        public override bool IsTerminal
        {
            get { return false; }
        }

        public ParameterList Parameters { get; set; }
        public AttributeList Attributes { get; set; }
        public StatementList Children { get; set; }
    }
}