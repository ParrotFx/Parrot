namespace Parrot.Mvc
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Web.Mvc;
    using Parrot.Infrastructure;
    using Parrot.Nodes;
    using Parrot.Renderers;
    using Parrot.Renderers.Infrastructure;

    public class ParrotView : IView
    {
        private readonly string _viewPath;
        private readonly IHost _host;

        public ParrotView(IHost host, string viewPath)
        {
            _viewPath = viewPath;
            _host = host;
        }

        #region IView Members

        internal Stream LoadStream()
        {
            var pathResolver = _host.PathResolver;
            return pathResolver.OpenFile(_viewPath);
        }

        public void Render(ViewContext viewContext, IParrotWriter writer)
        {
            //View contents
            using (var stream = LoadStream())
            {
                string contents = new StreamReader(stream).ReadToEnd();

                contents = Parse(viewContext, contents);

                string output = contents;

                writer.Write(output);
            }
        }

        public void Render(ViewContext viewContext, TextWriter writer)
        {
            var parrotWriter = _host.CreateWriter();

            Render(viewContext, parrotWriter);

            writer.Write(parrotWriter.Result());
        }

        internal Document LoadDocument(string template)
        {
            Parser.Parser parser = new Parser.Parser();
            Document document;

            if (parser.Parse(template, out document))
            {
                return document;
            }

            throw new Exception("Unable to parse: ");
        }

        private string Parse(ViewContext viewContext, string template)
        {
            Stopwatch watch = Stopwatch.StartNew();

            Document document = LoadDocument(template);

            object model = null;
            if (viewContext != null)
            {
                model = viewContext.ViewData.Model;
            }

            var documentHost = new Dictionary<string, object>();
            documentHost.Add("Model", model);
            if (viewContext != null)
            {
                documentHost.Add("Request", viewContext.RequestContext.HttpContext.Request);
                documentHost.Add("User", viewContext.RequestContext.HttpContext.User);
            }

            //need to create a custom viewhost
            var rendererFactory = _host.RendererFactory;
            DocumentView view = new DocumentView(_host, rendererFactory, documentHost, document);

            IParrotWriter writer = _host.CreateWriter();
            view.Render(writer);

            watch.Stop();

            return writer.Result();
            //+ "\r\n\r\n<!--\r\n" + template + "\r\n-->"
            //+ "\r\n\r\n<!--\r\n" + watch.Elapsed + "\r\n-->";
        }

        #endregion
    }
}