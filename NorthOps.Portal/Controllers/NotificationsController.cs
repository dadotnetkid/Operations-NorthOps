using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using NorthOps.Models.Repository;

namespace NorthOps.Portal.Controllers
{
    public class NotificationsController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        // GET: Notifications
        [ChildActionOnly]
        public ActionResult Index()
        {
            var UserId = User.Identity.GetUserId();
            var model = unitOfWork.EmployeeNoticationsRepo.Fetch(m => m.UserId == UserId).OrderBy(m => m.Id).Skip(0).Take( 5);
            return PartialView("_Notifications", model);
        }
    }
}