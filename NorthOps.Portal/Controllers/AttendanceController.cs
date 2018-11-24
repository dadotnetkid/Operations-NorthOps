using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using NorthOps.Models.Repository;

namespace NorthOps.Portal.Controllers
{
    public class AttendanceController : Controller
    {
        public string UserId => User.Identity.GetUserId();

        private UnitOfWork unitOfWork = new UnitOfWork();
        // GET: Attendance
        public ActionResult Index()
        {
            return View();
        }

        [ValidateInput(false)]
        public ActionResult RawAttendanceGridViewPartial()
        {
            var user = unitOfWork.UserRepository.Find(m => m.Id == UserId);
            var model = unitOfWork.AttendancesRepo.Get(m => m.BiometricId == user.BiometricId);
            return PartialView("_RawAttendanceGridViewPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RawAttendanceGridViewPartialAddNew(NorthOps.Models.Attendances item)
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
            return PartialView("_RawAttendanceGridViewPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult RawAttendanceGridViewPartialUpdate(NorthOps.Models.Attendances item)
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
            return PartialView("_RawAttendanceGridViewPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult RawAttendanceGridViewPartialDelete(System.Int32 Id)
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
            return PartialView("_RawAttendanceGridViewPartial", model);
        }
    }
}