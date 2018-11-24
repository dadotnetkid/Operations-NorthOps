using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using NorthOps.Models;
using NorthOps.Models.Repository;
using NorthOps.Services.AttendanceService;
using NorthOps.Services.DTRService;

namespace NorthOps.Portal.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        private string _userId;

        public string UserId
        {
            get
            {

                return User.Identity.GetUserId();

            }

        }

        public ActionResult Index()
        {
            if (User.IsInRole("Employee"))
            {
                return RedirectToAction("index", "DailyTimeRecord");
            }

            ViewBag.Notification = unitOfWork.EmployeeNoticationsRepo.Get(m => m.Id == UserId);
            return View();
        }
        [Authorize(Roles = "Administrator")]
        public ActionResult ApplicantDashboard(string UserId)
        {

            ViewBag.Notification = unitOfWork.EmployeeNoticationsRepo.Get(m => m.Id == UserId);
            ViewBag.UserId = UserId;
            return View("Index");
        }
        [HttpPost]
        public async Task<ActionResult> Index(string str = "")
        {
            for (var i = 1; i <= 70; i++)
            {
                Debug.WriteLine($"{i}:{i % 7}");
            }
            return View();
        }



        #region Applicant Dashboard


        [ValidateInput(false)]
        public ActionResult ApplicationStatusGridViewPartial(string UserId = "")
        {
            var model = new ApplicantStatusModel().applicantStatusModel(UserId);
            return PartialView("_ApplicationStatusGridViewPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ApplicationStatusGridViewPartialAddNew(ApplicantStatusModel item)
        {
            var model = new object[0];
            if (ModelState.IsValid)
            {
                try
                {
                    // Insert here a code to insert the new item in your model
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("_ApplicationStatusGridViewPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult ApplicationStatusGridViewPartialUpdate(ApplicantStatusModel item)
        {
            var model = new object[0];
            if (ModelState.IsValid)
            {
                try
                {
                    // Insert here a code to update the item in your model
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("_ApplicationStatusGridViewPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult ApplicationStatusGridViewPartialDelete(System.String UserId)
        {
            var model = new object[0];
            if (UserId != null)
            {
                try
                {
                    // Insert here a code to delete the item from your model
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            return PartialView("_ApplicationStatusGridViewPartial", model);
        }
        #endregion

        #region Employee Applicant

        [ValidateInput(false)]
        public async Task<ActionResult> AttendanceGridViewGridViewPartial()
        {

            var model = await new AttendanceServices().GetDtrReport(User.Identity.GetUserId());
            return PartialView("_AttendanceGridViewGridViewPartial", model.DtrReportViewModels);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult AttendanceGridViewGridViewPartialAddNew(NorthOps.Models.Attendances item)
        {
            var model = new object[0];
            if (ModelState.IsValid)
            {
                try
                {
                    // Insert here a code to insert the new item in your model
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("_AttendanceGridViewGridViewPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult AttendanceGridViewGridViewPartialUpdate()
        {
            if (ModelState.IsValid)
            {
                try
                {


                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("_AttendanceGridViewGridViewPartial");
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult AttendanceGridViewGridViewPartialDelete(System.Int32 Id)
        {
            var model = new object[0];
            if (Id >= 0)
            {
                try
                {
                    // Insert here a code to delete the item from your model
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            return PartialView("_AttendanceGridViewGridViewPartial", model);
        }


        #endregion

    }
}