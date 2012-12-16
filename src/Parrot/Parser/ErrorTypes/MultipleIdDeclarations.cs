namespace Parrot.Parser.ErrorTypes
{
    public class MultipleIdDeclarations : ParserError
    {
        public override string Message { get { return ""; } }
        public string Id { get; set; }

        public MultipleIdDeclarations(string id)
        {
            Id = id;
        }
    }
}