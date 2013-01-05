// -----------------------------------------------------------------------
// <copyright file="Document.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Parrot.Nodes
{
    using System.Collections.Generic;
    using Parrot.Parser.ErrorTypes;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Document
    {
        public IList<ParserError> Errors { get; set; }
        public StatementList Children { get; set; }

        public Document()
        {
            Children = new StatementList();
            Errors = new List<ParserError>();
        }

        public Document(Document document, Statement statement)
        {
            Children = document.Children;
            Children.Add(statement);
        }
    }
}