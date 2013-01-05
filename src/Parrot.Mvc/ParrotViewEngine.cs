namespace Parrot.Mvc
{
    using System.Web.Mvc;
    using Parrot.Renderers.Infrastructure;

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
            var pathResolver = _host.PathResolver;

            searchedLocations = new string[SearchLocations.Length];

            for (int i = 0; i < SearchLocations.Length; i++)
            {
                string virtualPath = string.Format(SearchLocations[i], viewName, controllerName);

                searchedLocations[i] = virtualPath;

                //check the active VirtualPathProvider if the file exists
                if (pathResolver.FileExists(virtualPath))
                {
                    //add it to cache - not currently implemented
                    return pathResolver.ResolvePath(virtualPath);
                }
            }

            return null;
        }

        public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            return FindPartialView(controllerContext, viewName, useCache);
        }

        public void ReleaseView(ControllerContext controllerContext, IView view)
        {
        }

        #endregion
    }
}