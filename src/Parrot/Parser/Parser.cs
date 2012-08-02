using System.IO;
using System.Text;
using GoldParser;
using Parrot.Nodes;

namespace Parrot.Parser
{
    public class Parser
    {
        ParserContext _context;
        string _errorString;
        GoldParser.Parser _parser;

        public Parser()
        {
            ParserFactory.InitializeFactoryFromResource("Parrot.parrot-with-unicode.cgt");
        }

        public int LineNumber
        {
            get
            {
                return _parser.LineNumber;
            }
        }

        public int LinePosition
        {
            get
            {
                return _parser.LinePosition;
            }
        }

        public string ErrorString
        {
            get
            {
                return _errorString;
            }
        }

        public string ErrorLine
        {
            get
            {
                return _parser.LineText;
            }
        }

        public bool Parse(string source, out Document document)
        {
            return Parse(new StringReader(source), out document);
        }

        public bool Parse(StringReader sourceReader, out Document document)
        {
            _parser = ParserFactory.CreateParser(sourceReader);
            _parser.TrimReductions = true;
            _context = new ParserContext(_parser);

            document = null;

            while (true)
            {
                switch (_parser.Parse())
                {
                    case ParseMessage.LexicalError:
                        _errorString = string.Format("Lexical Error. Line {0}. Token {1} was not expected.", _parser.LineNumber, _parser.TokenText);
                        return false;

                    case ParseMessage.SyntaxError:
                        StringBuilder text = new StringBuilder();
                        foreach (Symbol tokenSymbol in _parser.GetExpectedTokens())
                        {
                            text.Append(' ');
                            text.Append(tokenSymbol.ToString());
                        }
                        _errorString = string.Format("Syntax Error. Line {0}. Expecting: {1}.", _parser.LineNumber, text);

                        return false;
                    case ParseMessage.Reduction:
                        _parser.TokenSyntaxNode = _context.CreateASTNode();
                        break;

                    case ParseMessage.Accept:
                        var result = _parser.TokenSyntaxNode as BlockNodeList;
                        if (result == null)
                        {
                            result = new BlockNodeList(_parser.TokenSyntaxNode as BlockNode);
                        }

                        document = new Document
                        {
                            Children = result
                        };

                        _errorString = null;
                        return true;

                    case ParseMessage.TokenRead:
                        //=== Make sure that we store token string for needed tokens.
                        _parser.TokenSyntaxNode = _context.GetTokenText();
                        break;

                    case ParseMessage.InternalError:
                        _errorString = "Internal Error. Something is horribly wrong.";
                        return false;

                    case ParseMessage.NotLoadedError:
                        _errorString = "Grammar Table is not loaded.";
                        return false;

                    case ParseMessage.CommentError:
                        _errorString = "Comment Error. Unexpected end of input.";

                        return false;

                    case ParseMessage.CommentBlockRead:
                    case ParseMessage.CommentLineRead:
                        // don't do anything 
                        break;
                }
            }
        }

    }
}