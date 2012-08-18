// -----------------------------------------------------------------------
// <copyright file="Document.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using Parrot.Infrastructure;

namespace Parrot.Nodes
{
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Document
    {
        public StatementList Children;

        public Document()
        {
            Children = new StatementList();
        }

        public Document(Document document, Statement statement)
        {
            Children = document.Children;
            Children.Add(statement);
        }
    }
}