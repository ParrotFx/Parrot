namespace Parrot.Nodes
{
    public class Identifier
    {
        public string Name { get; set; }
        public int Index { get; set; }
        public int Length { get; set; }
        public IdentifierType Type { get; set; }

        public override string ToString()
        {
            return string.Format("{0}: {1}", Name, Type);
        }
    }
}