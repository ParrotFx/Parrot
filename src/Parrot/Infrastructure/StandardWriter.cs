// -----------------------------------------------------------------------
// <copyright file="StandardWriter.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System.Text;
using System.IO;

namespace Parrot.Infrastructure
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>

    public class StandardWriter : IParrotWriter
    {
        private readonly StringBuilder _sb;
        private readonly StringWriter _writer;
        public StandardWriter()
        {
            _sb = new StringBuilder();
            _writer  = new StringWriter(_sb);
        }

        public virtual void Write(string value)
        {
            _writer.Write(value);
        }

        public virtual string Result()
        {
            return _sb.ToString();
        }
    }
}
