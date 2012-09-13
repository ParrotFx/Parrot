// -----------------------------------------------------------------------
// <copyright file="ParserException.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using Parrot.Lexer;

namespace Parrot
{
    using System;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ParserException : Exception
    {
        public ParserException(string message) : base(message) { }

        public ParserException(Token token) : base(string.Format("Invalid token '{0}' at {1}", token.Type, token.Index)) { }
    }
}
