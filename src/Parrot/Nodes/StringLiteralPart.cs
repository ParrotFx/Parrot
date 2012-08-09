namespace Parrot.Nodes
{
    public class StringLiteralPart
    {
        public string Data { get; private set; }
        public StringLiteralPartType Type { get; private set; }
        public int Index { get; private set; }
        public int Length { get; private set; }

        public StringLiteralPart(StringLiteralPartType type, string data, int index)
        {
            Type = type;
            Data = data;
            Index = index;
            Length = data.Length;
        }
    }
}