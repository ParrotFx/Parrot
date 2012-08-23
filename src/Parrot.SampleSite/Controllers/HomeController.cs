using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Parrot.Infrastructure;
using Parrot.Mvc;

namespace Parrot.SampleSite.Controllers
{
    using System;
    using System.IO;
    using System.Text;
    using System.Web.Mvc;
    using Parrot;
    using Parrot.Nodes;
    using Renderers.Infrastructure;

    public class HomeController : Controller
    {
        //
        // GET: /Home/
        private readonly IHost _host;

        public HomeController(AspNetHost host)
        {
            _host = host;
        }

        public const string DefaultHtmlTemplate = @"div#firstname.nameInfo {
    label { 'First name' }
    span(FirstName)
}
div#lastname.nameInfo {
    label { 'Last name' }
    span(LastName)
}
div#age.nameInfo {
    label { 'Age' }
    span(Age)
}
div#address.addressInfo {
    div(Address) {
        div(StreetAddress)
        div(City)
        div(State)
        div(PostalCode)
    }
}
ul#phoneNumbers.phone(PhoneNumber) {
    li {
        span(Type)
        span(Number)
    }
}";
        private const string DefaultModelTemplate = @"{
     ""FirstName"": ""John"",
     ""LastName"" : ""Smith"",
     ""Age""      : 25,
     ""Address""  :
     {
         ""StreetAddress"": ""21 2nd Street"",
         ""City""         : ""New York"",
         ""State""        : ""NY"",
         ""PostalCode""   : ""10021""
     },
     ""PhoneNumber"":
     [
         {
           ""Type""  : ""home"",
           ""Number"": ""212 555-1234""
         },
         {
           ""Type""  : ""fax"",
           ""Number"": ""646 555-4567""
         }
     ]
 }";

        public ActionResult Child()
        {
            return View(new { Title = "BLAH BLAH BLAH" });
        }
        
        [ValidateInput(false)]
        public ActionResult Index(string template, string model)
        {
            if (string.IsNullOrWhiteSpace(template) || string.IsNullOrWhiteSpace(model))
            {
                return View(Tuple.Create(DefaultHtmlTemplate, DefaultModelTemplate, ""));
            }

            Parser.Parser parser = new Parser.Parser();

            Type T = TypeBuilderFromJson.CreateType(Newtonsoft.Json.JsonConvert.DeserializeObject(model) as JObject);

            var modelObject = Newtonsoft.Json.JsonConvert.DeserializeObject(model, T);

            Document document;
            string result = null;
            if (parser.Parse(new StringReader(template), _host, out document))
            {
                StringBuilder sb = new StringBuilder();
                foreach (var element in document.Children)
                {
                    if (element != null)
                    {
                        var renderer = _host.DependencyResolver.Get<IRendererFactory>().GetRenderer(element.Name);
                        sb.AppendLine(renderer.Render(element, modelObject));
                    }
                }

                result = sb.ToString();
            }
            else
            {
                //TODO: Get this later
                result = "Oops!"; // parser.ErrorString;
            }

            return View(Tuple.Create(template, model, result));
        }

        public ActionResult Hattan()
        {
            return View(CreateHattanObject());
        }

        private object CreateHattanObject()
        {
            return new List<HattanObject>
            {
                new HattanObject
                {
                    DatePosted= "July 26 - 11:51 AM",
                    Description = "Last night I presented at the San Francisco .NET Developers User Group on Knockout JS with ASP.NET MVC4 and Web API. I had a great time there and I wanted to thank everyone for inviting me to speak to the group. Here are my slides from the presentation. Here is the Contact List application we [...]",
                    PostId = 275,
                    Title = "Knockout JS @ The San Francisco .NET Developers User Group",
                    Url = "http://mvcdotnet.wordpress.com/2012/07/26/knockout-js-the-san-francisco-net-developers-user-group/"
                },
                new HattanObject()
                {
                    DatePosted= "July 23 - 12:03 AM",
                    Description = "In this article I’m going to show you how to build an ASP.NET MVC web application that will allow you to upload a CSV file and display the results in a Telerik Grid. We’re going to be using two libraries to help us with this task. LinqToCsv which is going to help us parse the [...]",
                    PostId = 275,
                    Title = "ASP.NET MVC– Display the contents of a CSV file in a Telerik Grid",
                    Url = "http://mvcdotnet.wordpress.com/2012/07/23/asp-net-mvc-display-the-contents-of-a-csv-file-in-a-telerik-grid/"
                },
                new HattanObject()
                {
                    DatePosted= "July 01 - 4:55 PM",
                    Description = "A friend recently emailed me and asked me for advice on learning the Razor syntax. I’ve decided to create this blog as a reply in order to benefit anyone else that might have the same question. Razor is a recent addition to the ASP.NET family and provides a new syntax for defining server side code [...]",
                    PostId = 275,
                    Title = "Getting started with Razor",
                    Url = "http://mvcdotnet.wordpress.com/2012/07/01/getting-started-with-razor/"
                },
                new HattanObject()
                {
                    DatePosted= "November 07 - 4:04 PM",
                    Description = "Tonight I’m presenting at the Los Angeles Dot Net user group (LADOTNET). I’ll be covering Knockout JS and how you can use it in an ASP.NET MVC app. I hope to see you there. Here are the slides and demos Slides Demo Source Code – Play with the Demos Online Here’s a few useful links about [...]",
                    PostId = 275,
                    Title = "Knockout JS Presentation",
                    Url = "http://mvcdotnet.wordpress.com/2011/11/07/knockout-js-presentation/"
                },
                new HattanObject()
                {
                    DatePosted= "October 28 - 12:49 PM",
                    Description = "This past Tuesday, I had the opportunity to speak at another local user group, SoCal Team System. My talk was a hands on introduction to unit testing. We went over several code demos and built a simple asp.net mvc site using TDD best practice. Thanks to everyone that came out! Here is the Slide Deck and [...]",
                    PostId = 275,
                    Title = "SoCal Team System Presentation",
                    Url = "http://mvcdotnet.wordpress.com/2011/10/28/socal-team-system-presentation/"
                },
                new HattanObject()
                {
                    DatePosted= "October 18 - 12:44 AM",
                    Description = "This past weekended I spoke at the 4th annual LA Code Camp. I had an amazing time speaking, listening to some great presentations and interacting with lots of great people. Thanks to everyone that came to my talks! I would really appreciate feed back on my presentations. So if you attended a talk, please visit my [...]",
                    PostId = 275,
                    Title = "SoCal Code Camp Los Angeles 2011",
                    Url = "http://mvcdotnet.wordpress.com/2011/10/18/socal-code-camp-los-angeles-2011/"
                },
                new HattanObject()
                {
                    DatePosted= "September 13 - 1:23 AM",
                    Description = "Later today I’ll be speaking at the San Luis Obispo .NET User group. I’ll be presenting on Knockout JS, a powerful MVVM library for JavaScript. Knockout makes building interactive rich UI’s a lot of fun! I hope to see you there. If you do attend, I ask that you please rate my talk so I [...]",
                    PostId = 275,
                    Title = "SLO .NET User Group Talk",
                    Url = "http://mvcdotnet.wordpress.com/2011/09/13/slo-net-user-group-talk/"
                },
                new HattanObject()
                {
                    DatePosted= "June 27 - 12:40 AM",
                    Description = "This past weekend I attended the SoCal Code Camp in San Diego and I had a blast. I gave 5 talks on mvc related topic and attended several great sessions. It was a fun experience and I truly enjoy going to Code Camp, it’s a great place to learn about new technologies and hang out [...]",
                    PostId = 275,
                    Title = "SoCal Code Camp San Diego 2011",
                    Url = "http://mvcdotnet.wordpress.com/2011/06/27/socal-code-camp-san-diego-2011/"
                },
                new HattanObject()
                {
                    DatePosted= "April 12 - 2:41 PM",
                    Description = "Today Microsoft released a new add-on for ASP.NET MVC 3. The ASP.NET MVC3 Tools Update does not update any of the dll’s used by an mvc3 project, but adds some great new tooling support. You can read more about in on Phil Haack’s Blog Post regarding this release. In this post I’m going to show [...]",
                    PostId = 275,
                    Title = "ASP.NET MVC3 Tools Update – Scaffolding with the Repository Pattern",
                    Url = "http://mvcdotnet.wordpress.com/2011/04/12/asp-net-mvc3-tools-update-scaffolding-with-the-repository-pattern/"
                },
                new HattanObject()
                {
                    DatePosted= "April 01 - 6:06 PM",
                    Description = "In asp.net mvc when you add a view to correspond with an action method, you have the option of using several pre-defined templates that come with asp.net mvc (Create, Edit, Delete, Details and List.) Depending on the selected template, a particular view is scaffolded for you containing all the markup needed to perform the action. [...]",
                    PostId = 275,
                    Title = "Dynamic Header names in the default ASP.NET MVC List Template",
                    Url = "http://mvcdotnet.wordpress.com/2011/03/31/dynamic-header-in-default-asp-net-mvc-list-template/"
                },
            };
        }
    }

    public class HattanObject
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public string DatePosted { get; set; }
        public string Description { get; set; }
        public int PostId { get; set; }
    }

    public class SampleObject
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public Address Address { get; set; }
        public PhoneNumber[] PhoneNumber { get; set; }
    }

    public class PhoneNumber
    {
        public string Type { get; set; }
        public string Number { get; set; }
    }

    public class Address
    {
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
    }

}
