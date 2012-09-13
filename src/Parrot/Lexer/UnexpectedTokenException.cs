using System;

namespace Parrot.Lexer
{
    public class UnexpectedTokenException : Exception
    {
        public UnexpectedTokenException(string mesage) : base(mesage)
        {
        }
    }
}