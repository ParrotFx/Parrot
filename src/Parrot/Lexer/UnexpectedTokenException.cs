namespace Parrot.Lexer
{
    using System;

    public class UnexpectedTokenException : Exception
    {
        public UnexpectedTokenException(string mesage) : base(mesage)
        {
        }
    }
}