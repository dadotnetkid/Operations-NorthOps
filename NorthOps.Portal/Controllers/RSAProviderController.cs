using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RestSharp;

namespace NorthOps.Portal.Controllers
{
    public class RSAProviderController : Controller
    {
        public ActionResult Index(string xml)
        {
            
            //   var verToken = htmlDocument.DocumentNode.SelectNodes("//meta").First().Attributes["content"].Value;
            return PartialView("index",xml);
        }

        public ActionResult Test()
        {
            return View();
        }
    }
}