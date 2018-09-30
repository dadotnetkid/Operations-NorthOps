using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NorthOps.Internals.Helpers;

namespace NorthOps.Internals.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var directoryHelpers = new ActiveDirectoryHelpers();
            directoryHelpers.CreateUser(new Models.Users(){UserName="MarkCacal",Email="mark@gmail.com",Password="n0rth@dm1N"});
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}