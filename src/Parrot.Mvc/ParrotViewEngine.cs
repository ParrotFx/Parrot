namespace Parrot.Mvc
{
    using System;
    using System.IO;
    using System.Web.Hosting;
    using System.Web.Mvc;
    using Nodes;
    using System.Diagnostics;
    using Renderers;

    public class ParrotViewEngine : IViewEngine
    {
        #region IViewEngine Members

        public static IRendererFactory Factory;

        static ParrotViewEngine()
        {
            RendererFactory factory = new RendererFactory();

            factory.RegisterFactory(new[] { "base", "basefont", "frame", "link", "meta", "area", "br", "col", "hr", "img", "param" }, new SelfClosingRenderer());
            factory.RegisterFactory("doctype", new DocTypeRenderer());
            factory.RegisterFactory("rawoutput", new RawOutputRenderer());
            factory.RegisterFactory("output", new OutputRenderer());
            factory.RegisterFactory("input", new InputRenderer());
            factory.RegisterFactory("string", new StringLiteralRenderer());
            factory.RegisterFactory("layout", new LayoutRenderer());
            factory.RegisterFactory("content", new ContentRenderer());

            //default renderer
            factory.RegisterFactory("*", new HtmlRenderer());

            Infrastructure.Host.DependencyResolver.Register(typeof(IRendererFactory), () => factory);

            Factory = factory;
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
            return new ParrotView(viewPath);
        }

        public string FindPath(string viewName, string controllerName, out string[] searchedLocations)
        {
            searchedLocations = new string[SearchLocations.Length];

            for (int i = 0; i < SearchLocations.Length; i++)
            {
                string virtualPath = string.Format(SearchLocations[i], viewName, controllerName);

                searchedLocations[i] = virtualPath;

                //check the active VirtualPathProvider if the file exists
                if (HostingEnvironment.VirtualPathProvider.FileExists(virtualPath))
                {
                    //add it to cache - not currently implemented
                    return HostingEnvironment.VirtualPathProvider.GetFile(virtualPath).VirtualPath;
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

        public ParrotView(string viewPath)
        {
            this._viewPath = viewPath;
        }

        #region IView Members

        internal Stream LoadStream()
        {
            return VirtualPathProvider.OpenFile(_viewPath);
        }

        public void Render(ViewContext viewContext, TextWriter writer)
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

        internal static Document LoadDocument(string template)
        {
            Parser.Parser parser = new Parser.Parser();
            Document document;

            if (parser.Parse(new StringReader(template), out document))
            {
                return document;
            }

            throw new Exception("Unable to parse: ");
            //throw new Exception("Unable to parse: " + parser.ErrorString);
        }
        
        string Parse(ViewContext viewContext, string template)
        {
            string result;

            Stopwatch watch = Stopwatch.StartNew();
            try
            {
                Document document = LoadDocument(template);

                result = document.Render(viewContext.ViewData.Model);
            }
            catch (Exception e)
            {
                result = e.Message;
            }

            watch.Stop();

            return result
                + "\r\n\r\n<!--\r\n" + template + "\r\n-->"
                + "\r\n\r\n<!--\r\n" + watch.Elapsed + "\r\n-->";
        }

        #endregion
    }

}