using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NorthOps.Portal.Controllers
{

    public class ErrorHandlerController : Controller
    {
        // GET: ErrorHandler
        [HandleError]
        public ActionResult Index()
        {
            return View();
        }
    }
}