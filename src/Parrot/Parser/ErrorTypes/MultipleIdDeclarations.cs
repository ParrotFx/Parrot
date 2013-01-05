namespace Parrot.Parser.ErrorTypes
{
    public class MultipleIdDeclarations : ParserError
    {
        public override string Message
        {
            get { return "Element may not have more than one Id"; }
        }

        public string Id { get; set; }

        public MultipleIdDeclarations(string id)
        {
            Id = id;
        }
    }
}