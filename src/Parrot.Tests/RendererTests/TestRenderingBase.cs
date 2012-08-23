// -----------------------------------------------------------------------
// <copyright file="TestRenderingBase.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System.IO;
using Parrot.Infrastructure;
using Parrot.Mvc.Renderers;
using Parrot.Nodes;

namespace Parrot.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class TestRenderingBase
    {
        protected string Render(string parrot, object model, IHost host)
        {
            Parser.Parser parser = new Parser.Parser();
            Document document;

            parser.Parse(new StringReader(parrot), host, out document);

            DocumentRenderer renderer = new DocumentRenderer(new MemoryHost());

            StringBuilder sb = new StringBuilder();
            return renderer.Render(document, model);
        }

        protected string Render(string parrot, object model)
        {
            return Render(parrot, model, new MemoryHost());
        }

        protected string Render(string parrot)
        {
            return Render(parrot, null, new MemoryHost());
        }

    }
}
