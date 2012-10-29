namespace Parrot.Infrastructure
{
    public class PrettyStringWriter : StandardWriter
    {
        private int _indentation;
        private PrettyPrintWriteType _lastWritten = PrettyPrintWriteType.None;

        public PrettyStringWriter() : base()
        {
            _indentation = 0;
        }

        private PrettyPrintWriteType GetType(string value)
        {
            if (value.EndsWith("/>"))
            {
                //closing
                return PrettyPrintWriteType.SelfClosingElement;
            }
            
            if (value.StartsWith("</") && value.EndsWith(">"))
            {
                return PrettyPrintWriteType.ClosingElement;
            }
            
            if (value.StartsWith("<") && value.EndsWith(">"))
            {
                return PrettyPrintWriteType.OpeningElement;
            }

            return PrettyPrintWriteType.Literal;
        }

        private enum PrettyPrintWriteType
        {
            None,
            SelfClosingElement,
            ClosingElement,
            OpeningElement,
            Literal
        }


        public override void Write(string value)
        {
            var incrementIndentation = 0;
            var addNewLineAfterWrite = false;

            //check type
            var type = GetType(value);
            switch (type)
            {
                case PrettyPrintWriteType.SelfClosingElement:
                    incrementIndentation = 0;
                    addNewLineAfterWrite = true;
                    break;
                case PrettyPrintWriteType.ClosingElement:
                    if (_lastWritten == PrettyPrintWriteType.Literal)
                    {
                        base.Write("\r\n");
                    }
                    _indentation -=1;
                    addNewLineAfterWrite = true;
                    break;
                case PrettyPrintWriteType.OpeningElement:
                    incrementIndentation = 1;
                    addNewLineAfterWrite = true;
                    break;
                case PrettyPrintWriteType.Literal:
                    if (_lastWritten == PrettyPrintWriteType.OpeningElement)
                    {
                        //incrementIndentation = 1;
                    }
                    break;
            }

            if (_indentation > 0 && (_lastWritten != PrettyPrintWriteType.Literal || (_lastWritten == PrettyPrintWriteType.Literal && type == PrettyPrintWriteType.ClosingElement)))
            {
                base.Write(new string('\t', _indentation));
            }
            _indentation += incrementIndentation;

            base.Write(value);
            _lastWritten = type;

            if (addNewLineAfterWrite)
            {
                base.Write("\r\n");
            }


        }

    }
}