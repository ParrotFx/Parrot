// -----------------------------------------------------------------------
// <copyright file="View.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System.IO;
using Parrot.Infrastructure;
using Parrot.Nodes;
using Parrot.Renderers.Infrastructure;

namespace Parrot.Renderers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class DocumentView
    {
        protected IDictionary<string, object> DocumentHost;
        private readonly IRendererFactory _rendererFactory;
        private readonly IHost _host;
        private readonly Document _document;

        public DocumentView(IHost host, IRendererFactory rendererFactory, IDictionary<string, object> documentHost, Document document)
        {
            DocumentHost = documentHost;
            _host = host;
            _rendererFactory = rendererFactory;
            _document = document;
        }

        public void Render(IParrotWriter writer)
        {
            foreach (var element in _document.Children)
            {
                var renderer = _rendererFactory.GetRenderer(element.Name);
                renderer.Render(writer, element, DocumentHost, DocumentHost.GetValueOrDefault("Model"));
            }
        }
    }
}
