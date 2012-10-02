// -----------------------------------------------------------------------
// <copyright file="StringLiteralPipe.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using Parrot.Infrastructure;

namespace Parrot.Nodes
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class StringLiteralPipe : StringLiteral
    {
        public StringLiteralPipe(IHost host, string value) : this(host, value, null) { }

        public StringLiteralPipe(IHost host, string value, StatementTail statementTail) : base(host, "\"" + value + "\"", statementTail) { }
    }
}
