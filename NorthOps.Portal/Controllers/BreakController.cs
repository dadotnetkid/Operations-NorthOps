using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using NorthOps.Models;
using NorthOps.Models.Repository;
using NorthOps.Services.Helpers;

namespace NorthOps.Portal.Controllers
{
    [Authorize(Roles = "Employee")]
    public class BreakController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();

        private string UserId => User.Identity.GetUserId();
        // GET: Break
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult btnBreakInOut()
        {
            var dateFrom = Convert.ToDateTime($"{DateTime.Now.ToShortDateString()} 00:00");
            var dateTo = Convert.ToDateTime($"{DateTime.Now.ToShortDateString()} 23:59");

            var res = unitOfWork.BreaksRepo.Fetch(m => m.UserId == UserId).Where(m => m.DateCreated >= dateFrom && m.DateCreated <= dateTo)
                .OrderByDescending(m => m.DateCreated).FirstOrDefault();

            return PartialView("_btnBreakInOut", res);
        }

        public ActionResult BreakInOutPartial(int? BreakTypeId, BreakInOut breakInOut)
        {
            var dateFrom = Convert.ToDateTime($"{DateTime.Now.ToShortDateString()} 00:00");
            var dateTo = Convert.ToDateTime($"{DateTime.Now.ToShortDateString()} 23:59");

            var res = unitOfWork.BreaksRepo.Fetch(m => m.UserId == UserId).Where(m => m.DateCreated >= dateFrom && m.DateCreated <= dateTo)
                .OrderByDescending(m => m.DateCreated).FirstOrDefault();
            if (res == null)
            {
                unitOfWork.BreaksRepo.Insert(new Models.Breaks()
                {
                    BreakTypeId = BreakTypeId,
                    StartTime = DateTime.Now,
                    UserId = UserId,
                    DateCreated = DateTime.Now
                });

            }
            else if (res.EndTime == null)
            {
                res.EndTime = DateTime.Now;
            }
            else if (breakInOut == BreakInOut.Out)
            {
                unitOfWork.BreaksRepo.Insert(new Models.Breaks()
                {
                    BreakTypeId = BreakTypeId,
                    StartTime = DateTime.Now,
                    UserId = UserId,
                    DateCreated = DateTime.Now
                });
            }

            unitOfWork.Save();

            return btnBreakInOut();
        }


        [ValidateInput(false)]
        public ActionResult BreaksGridViewPartial()
        {
            var model = unitOfWork.BreaksRepo.Fetch(includeProperties: "Users,BreakTypes" );
            if (!User.IsInRoles("Team Leader", "Administrator"))
            {
                model = model.Where(m => m.UserId == UserId);
            }
            return PartialView("_BreaksGridViewPartial", model.OrderByDescending(m => m.DateCreated).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult BreaksGridViewPartialAddNew([ModelBinder(typeof(DevExpressEditorsBinder))]NorthOps.Models.Breaks item)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    unitOfWork.BreaksRepo.Insert(item);
                    unitOfWork.Save();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            var model = unitOfWork.BreaksRepo.Get(includeProperties: "Users,BreakTypes");
            return PartialView("_BreaksGridViewPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult BreaksGridViewPartialUpdate([ModelBinder(typeof(DevExpressEditorsBinder))]NorthOps.Models.Breaks item)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    unitOfWork.BreaksRepo.Update(item);
                    unitOfWork.Save();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            var model = unitOfWork.BreaksRepo.Get(includeProperties: "Users,BreakTypes");
            return PartialView("_BreaksGridViewPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult BreaksGridViewPartialDelete(System.Int32 Id)
        {
            if (Id >= 0)
            {
                try
                {
                    unitOfWork.BreaksRepo.Delete(m => m.Id == Id);
                    unitOfWork.Save();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            var model = unitOfWork.BreaksRepo.Get(includeProperties: "Users,BreakTypes");
            return PartialView("_BreaksGridViewPartial", model);
        }
    }
}