// -----------------------------------------------------------------------
// <copyright file="StringLiteralPipe.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Parrot.Nodes
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class StringLiteralPipe : StringLiteral
    {
        public StringLiteralPipe(string value) : this(value, null)
        {
        }

        public StringLiteralPipe(string value, StatementTail statementTail) : base("\"" + value + "\"", statementTail)
        {
        }
    }
}