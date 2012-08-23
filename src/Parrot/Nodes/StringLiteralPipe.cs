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
        public StringLiteralPipe(IHost host, string value) : base(host, "\"" + value + "\"")
        {
            
        }
    }
}
