namespace Parrot.Infrastructure
{
    public class PrettyStringWriter : StandardWriter
    {
        private int _indentation;
        private WriteType _lastWritten = WriteType.None;

        public PrettyStringWriter() : base()
        {
            _indentation = 0;
        }

        private WriteType GetType(string value)
        {
            if (value.EndsWith("/>"))
            {
                //closing
                return WriteType.SelfClosingElement;
            }
            
            if (value.StartsWith("</") && value.EndsWith(">"))
            {
                return WriteType.ClosingElement;
            }
            
            if (value.StartsWith("<") && value.EndsWith(">"))
            {
                return WriteType.OpeningElement;
            }

            return WriteType.Literal;
        }

        private enum WriteType
        {
            None,
            SelfClosingElement,
            ClosingElement,
            OpeningElement,
            Literal
        }


        public override void Write(string value)
        {
            var elementType = 0;
            var incrementIndentation = 0;
            var addNewLineAfterWrite = false;

            //check type
            var type = GetType(value);
            switch (type)
            {
                case WriteType.SelfClosingElement:
                    incrementIndentation = 0;
                    addNewLineAfterWrite = true;
                    break;
                case WriteType.ClosingElement:
                    if (_lastWritten == WriteType.Literal)
                    {
                        base.Write("\r\n");
                    }
                    _indentation -=1;
                    addNewLineAfterWrite = true;
                    break;
                case WriteType.OpeningElement:
                    incrementIndentation = 1;
                    addNewLineAfterWrite = true;
                    break;
                case WriteType.Literal:
                    if (_lastWritten == WriteType.OpeningElement)
                    {
                        //incrementIndentation = 1;
                    }
                    break;
            }

            if (_indentation > 0 && (_lastWritten != WriteType.Literal || (_lastWritten == WriteType.Literal && type == WriteType.ClosingElement)))
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