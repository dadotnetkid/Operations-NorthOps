using NorthOps.Models;
using NorthOps.Models.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NorthOps.Ops.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class ApplicantController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        // GET: Applicant
        [Route("dashboard/{UserId}")]
        public ActionResult Index(string UserId)
        {
            ViewBag.Notification = unitOfWork.EmployeeNoticationsRepo.Get(m => m.Id == UserId);
            ViewBag.UserId = UserId;
            return View();
        }
       public ActionResult ApplicantDashboardViewPartial(string UserId)
        {
            var model = new ApplicantStatusModel().applicantStatusModel(UserId);
            return PartialView("ApplicantDashboardViewPartial",model);
        }
    }
}