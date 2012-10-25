using System.Collections.Generic;
using System.Text;
using Parrot.Infrastructure;

namespace Parrot.Mvc
{
    using System;
    using System.IO;
    using System.Web.Mvc;
    using Nodes;
    using System.Diagnostics;
    using Parrot.Renderers;
    using Parrot.Renderers.Infrastructure;
    using Renderers;

    public class ParrotViewEngine : IViewEngine
    {
        #region IViewEngine Members

        private readonly IHost _host;

        public ParrotViewEngine(IHost host)
        {
            _host = host;
        }

        public readonly string[] SearchLocations = new[]
        {
            "~/Views/{1}/{0}.parrot",
            "~/Views/Shared/{0}.parrot"
        };

        public ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            //grab the current controller from the route data
            string controllerName = null;
            if (controllerContext != null)
            {
                controllerName = controllerContext.RouteData.GetRequiredString("controller");
            }

            //for proper error handling we need to return a list of locations we attempted to search for the view
            string[] searchedLocations;

            var result = FindView(partialViewName, controllerName, out searchedLocations);
            if (result != null)
            {
                return result;
            }

            //we couldn't find the view
            return new ViewEngineResult(searchedLocations);
        }

        private ViewEngineResult FindView(string viewName, string controllerName, out string[] searchedLocations)
        {
            ViewEngineResult viewEngineResult = null;

            //get the actual path of the view - returns null if none is found
            string viewPath = FindPath(viewName, controllerName, out searchedLocations);

            if (viewPath != null)
            {
                var view = GetView(viewPath);

                {
                    viewEngineResult = new ViewEngineResult(view, this);
                }
            }

            return viewEngineResult;
        }

        internal ParrotView GetView(string viewPath)
        {
            return new ParrotView(_host, viewPath);
        }

        public string FindPath(string viewName, string controllerName, out string[] searchedLocations)
        {
            var pathResolver = _host.DependencyResolver.Resolve<IPathResolver>();

            searchedLocations = new string[SearchLocations.Length];

            for (int i = 0; i < SearchLocations.Length; i++)
            {
                string virtualPath = string.Format(SearchLocations[i], viewName, controllerName);

                searchedLocations[i] = virtualPath;

                //check the active VirtualPathProvider if the file exists
                if (pathResolver.FileExists(virtualPath))
                {
                    //add it to cache - not currently implemented
                    return pathResolver.VirtualFilePath(virtualPath);
                }
            }

            return null;
        }

        public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            return FindPartialView(controllerContext, viewName, useCache);
        }

        public void ReleaseView(ControllerContext controllerContext, IView view) { }

        #endregion
    }

    public class ParrotView : IView
    {
        readonly string _viewPath;
        private readonly IHost _host;

        public ParrotView(IHost host,  string viewPath)
        {
            this._viewPath = viewPath;
            _host = host;
        }

        #region IView Members

        internal Stream LoadStream()
        {
            var pathResolver = _host.DependencyResolver.Resolve<IPathResolver>();
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
            var parrotWriter = _host.DependencyResolver.Resolve<IParrotWriter>();

            Render(viewContext, parrotWriter);

            writer.Write(parrotWriter.Result());
        }

        internal Document LoadDocument(string template)
        {
            Parser.Parser parser = new Parser.Parser(_host);
            Document document;

            if (parser.Parse(template, out document))
            {
                return document;
            }

            throw new Exception("Unable to parse: ");
        }

        string Parse(ViewContext viewContext, string template)
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
            var rendererFactory = _host.DependencyResolver.Resolve<IRendererFactory>();
            DocumentView view = new DocumentView(_host, rendererFactory, documentHost, document);

            IParrotWriter writer = _host.DependencyResolver.Resolve<IParrotWriter>();
            view.Render(writer);

            watch.Stop();

            return writer.Result();
            //+ "\r\n\r\n<!--\r\n" + template + "\r\n-->"
            //+ "\r\n\r\n<!--\r\n" + watch.Elapsed + "\r\n-->";
        }

        #endregion
    }

}