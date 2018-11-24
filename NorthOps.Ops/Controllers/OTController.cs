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
    [Authorize(Roles = "Administrator")]
    public class OTController : Controller
    {
        public string UserId => User.Identity.GetUserId();

        private UnitOfWork unitOfWork = new UnitOfWork();
        public ActionResult Index()
        {
            return View();
        }

        #region GridView
        [ValidateInput(false)]
        public ActionResult OTGridViewPartial()
        {
            var model = unitOfWork.OvertimesRepo.Get(includeProperties: "Users");
            return PartialView("_OTGridViewPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult OTGridViewPartialAddNew([ModelBinder(typeof(DevExpressEditorsBinder))] NorthOps.Models.Overtimes item)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    item.DateCreated = DateTime.Now;
                    item.Id = Guid.NewGuid().ToString();
                    unitOfWork.OvertimesRepo.Insert(item);
                    unitOfWork.Save();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            var model = unitOfWork.OvertimesRepo.Get(includeProperties: "Users");
            return PartialView("_OTGridViewPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult OTGridViewPartialUpdate([ModelBinder(typeof(DevExpressEditorsBinder))] NorthOps.Models.Overtimes item)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    unitOfWork.OvertimesRepo.Update(item);
                    unitOfWork.Save();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            var model = unitOfWork.OvertimesRepo.Get(includeProperties: "Users");
            return PartialView("_OTGridViewPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult OTGridViewPartialDelete([ModelBinder(typeof(DevExpressEditorsBinder))] Overtimes item)
        {

            if (item.Id != null)
            {
                try
                {
                    unitOfWork.OvertimesRepo.Delete(unitOfWork.OvertimesRepo.Find(m => m.Id == item.Id));
                    unitOfWork.Save();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            var model = unitOfWork.OvertimesRepo.Get(includeProperties: "Users");
            return PartialView("_OTGridViewPartial", model);
        }


        #endregion

        public ActionResult AddEditOvertimePartial([ModelBinder(typeof(DevExpressEditorsBinder))] Overtimes item)
        {
            var model = unitOfWork.OvertimesRepo.Find(m => m.Id == item.Id);
            return PartialView("_AddEditOvertimePartial", model);
        }
    }
}