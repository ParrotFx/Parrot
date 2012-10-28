// -----------------------------------------------------------------------
// <copyright file="SharpToken.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Parrot.Lexer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    internal class AtToken : Token
    {
        public AtToken()
        {
            Content = "@";
            Type = TokenType.At;
        }
}
}
