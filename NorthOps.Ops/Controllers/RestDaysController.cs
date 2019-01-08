using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using NorthOps.Models;
using NorthOps.Models.Repository;

namespace NorthOps.Ops.Controllers
{
    public class RestDaysController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        public ActionResult Index()
        {
            return View();
        }

        [ValidateInput(false)]
        public ActionResult RestDaysGridViewPartial()
        {
            var model = unitOfWork.RestDaysRepo.Get(includeProperties: "Users");
            return PartialView("_RestDaysGridViewPartial", model);
        }


        [ValidateAntiForgeryToken]
        [HttpPost, ValidateInput(false)]
        public ActionResult RestDaysGridViewPartialAddNew(NorthOps.Models.RestDays item)
        {
            var token = Request.Params["__RequestVerificationToken"];
            if (ModelState.IsValid)
            {
                try
                {
                    item.CreatedBy = User.Identity.GetUserId();
                    item.DateCreated = DateTime.Now;
                    item.ModifiedBy = User.Identity.GetUserId();

                    unitOfWork.RestDaysRepo.Insert(item);
                    unitOfWork.Save();

                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            var model = unitOfWork.RestDaysRepo.Get(includeProperties: "Users");
            return PartialView("_RestDaysGridViewPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult RestDaysGridViewPartialUpdate(NorthOps.Models.RestDays item)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    item.ModifiedBy = User.Identity.GetUserId();
                    unitOfWork.RestDaysRepo.Update(item);
                    unitOfWork.Save();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            var model = unitOfWork.RestDaysRepo.Get(includeProperties: "Users");
            return PartialView("_RestDaysGridViewPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult RestDaysGridViewPartialDelete(int? Id)
        {

            if (Id >= 0)
            {
                try
                {
                    unitOfWork.RestDaysRepo.Delete(m => m.Id == Id);
                    unitOfWork.Save();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            var model = unitOfWork.RestDaysRepo.Get(includeProperties: "Users");
            return PartialView("_RestDaysGridViewPartial", model);
        }


        public ActionResult AddEditRestDayPartial(int? restDayId)
        {
           

            var model = unitOfWork.RestDaysRepo.Find(m => m.Id == restDayId);
            return PartialView("_AddEditRestDayPartial", model);
        }
    }
}