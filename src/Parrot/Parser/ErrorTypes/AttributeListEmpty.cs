﻿namespace Parrot.Parser.ErrorTypes
{
    public class AttributeListEmpty : ParserError
    {
        public override string Message
        {
            get { return "Attribute list cannot be empty"; }
        }
    }
}