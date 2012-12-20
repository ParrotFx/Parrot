using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parrot.Parser.ErrorTypes
{
    public class AttributeValueMissing : ParserError
    {
        public override string Message { get { return "Attribute must have a value"; } }
    }
}
