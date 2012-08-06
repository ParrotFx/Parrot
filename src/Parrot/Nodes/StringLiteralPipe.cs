// -----------------------------------------------------------------------
// <copyright file="StringLiteralPipe.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Parrot.Nodes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class StringLiteralPipe : StringLiteral
    {
        public StringLiteralPipe(string value) : base("\"" + value + "\"")
        {
            
        }
    }
}
