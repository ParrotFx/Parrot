// -----------------------------------------------------------------------
// <copyright file="Document.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using Parrot.Infrastructure;
using Parrot.Lexer;
using Parrot.Parser.ErrorTypes;

namespace Parrot.Nodes
{
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Document
    {
        public IList<ParserError> Errors { get; set; }
        public StatementList Children { get; set; }

        public Document(IHost host)
        {
            Children = new StatementList(host);
            Errors = new List<ParserError>();
        }

        public Document(IHost host, Document document, Statement statement) : this(host)
        {
            Children = document.Children;
            Children.Add(statement);
        }
    }
}