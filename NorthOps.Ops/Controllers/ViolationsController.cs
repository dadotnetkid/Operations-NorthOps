using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using NorthOps.Models.Repository;

namespace NorthOps.Ops.Controllers
{
    public class ViolationsController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        public ActionResult Index()
        {
            return View();
        }

        [ValidateInput(false)]
        public ActionResult ViolationGridViewPartial()
        {
            var model = unitOfWork.ViolationsRepo.Get(includeProperties: "Users");
            return PartialView("_ViolationGridViewPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ViolationGridViewPartialAddNew(NorthOps.Models.Violations item)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    item.CreatedBy = User.Identity.GetUserId();
                    item.ModifiedBy = User.Identity.GetUserId();
                    unitOfWork.ViolationsRepo.Insert(item);
                    unitOfWork.Save();
                }

                catch (DbEntityValidationException ex)
                {
                    ViewData["EditError"] = ex.Message;
                    foreach (var i in ex?.EntityValidationErrors?.FirstOrDefault()?.ValidationErrors)
                    {
                        ModelState.AddModelError(i.PropertyName, i.ErrorMessage);
                    }

                    ViewData["Model"] = item;
                }
                catch (DbUpdateException ex)
                {
                    ViewData["EditError"] = ex.Message;
                    ViewData["Model"] = item;
                }
            }
            else
            {
                ViewData["EditError"] = "Please, correct all errors.";
                foreach (var i in ModelState.Where(x=>x.Value.Errors.Any()))
                {
                    ModelState.AddModelError(i.Key, i.Value?.Errors?.FirstOrDefault()?.ErrorMessage);
                }
                ViewData["Model"] = item;
            }
                
            var model = unitOfWork.ViolationsRepo.Get(includeProperties: "Users");
            return PartialView("_ViolationGridViewPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult ViolationGridViewPartialUpdate(NorthOps.Models.Violations item)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    item.ModifiedBy = User.Identity.GetUserId();
                    unitOfWork.ViolationsRepo.Update(item);
                    unitOfWork.Save();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            var model = unitOfWork.ViolationsRepo.Get(includeProperties: "Users");
            return PartialView("_ViolationGridViewPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult ViolationGridViewPartialDelete(System.Int32 Id)
        {

            if (Id >= 0)
            {
                try
                {
                    unitOfWork.ViolationsRepo.Delete(m => m.Id == Id);
                    unitOfWork.Save();

                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            var model = unitOfWork.ViolationsRepo.Get(includeProperties: "Users");
            return PartialView("_ViolationGridViewPartial", model);
        }

        public ActionResult AddEditViolationPartial(int? violationId)
        {
            var model = unitOfWork.ViolationsRepo.Find(m => m.Id == violationId, includeProperties: "Users");
            return PartialView("_AddEditViolationPartial",model);
        }

        public ActionResult cboUserPartial(string userId)
        {
            ViewBag.UserId = userId;
            var model = unitOfWork.UserRepository.Get(m=>m.UserRoles.Any(x=>x.Name=="Employee"));
            return PartialView("_cboUserPartial",model);
        }

        public ActionResult cboViolationTypePartial(string violationTypeId)
        {
            var model = unitOfWork.ViolationTypesRepo.Get();
            ViewBag.ViolationTypeId = violationTypeId;
            return PartialView("_cboViolationTypePartial",model);
        }
    }
}