using System;
using System.Runtime.Serialization;

namespace Parrot.Parser
{
    [Serializable]
    public class SymbolException : Exception
    {
        public SymbolException(string message) : base(message)
        {
        }

        public SymbolException(string message, Exception inner) : base(message, inner)
        {
        }

        protected SymbolException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

    }
}