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

        public Document(IHost host)
        {
            Children = new StatementList(host);
        }

        public Document(IHost host, Document document, Statement statement) : this(host)
        {
            Children = document.Children;
            Children.Add(statement);
        }
    }
}