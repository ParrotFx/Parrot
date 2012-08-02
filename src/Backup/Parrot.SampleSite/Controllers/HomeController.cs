using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Parrot;
using Parrot.Nodes;

namespace MvcApplication1.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        private const string HtmlTemplate = @"div#firstname.nameInfo {
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
        private const string ModelTemplate = @"{
     ""firstName"": ""John"",
     ""lastName"" : ""Smith"",
     ""age""      : 25,
     ""address""  :
     {
         ""streetAddress"": ""21 2nd Street"",
         ""city""         : ""New York"",
         ""state""        : ""NY"",
         ""postalCode""   : ""10021""
     },
     ""phoneNumber"":
     [
         {
           ""type""  : ""home"",
           ""number"": ""212 555-1234""
         },
         {
           ""type""  : ""fax"",
           ""number"": ""646 555-4567""
         }
     ]
 }";


        public ActionResult Index(string template, string model)
        {
            if (string.IsNullOrWhiteSpace(template) || string.IsNullOrWhiteSpace(model))
            {
                return View(Tuple.Create(HtmlTemplate, ModelTemplate, ""));
            }

            ParrotParser parser = new ParrotParser();

            SampleObject modelObject = Newtonsoft.Json.JsonConvert.DeserializeObject<SampleObject>(model);

            HtmlElementNodeList htmlElements;
            string result = null;
            if (parser.Parse(template, out htmlElements))
            {
                htmlElements.SetModel(modelObject);
                result = htmlElements.ToString();
            }
            else
            {
                result = parser.ErrorString;
            }

            return View(Tuple.Create(template, model, result));
        }

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
