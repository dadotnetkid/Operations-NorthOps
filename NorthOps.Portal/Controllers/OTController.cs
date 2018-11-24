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
    [Authorize(Roles = "Employee")]
    public class OTController : Controller
    {
        public string UserId => User.Identity.GetUserId();

        private UnitOfWork unitOfWork = new UnitOfWork();
        [Route("employee/overtime")]
        public ActionResult Index()
        {
            return View();
        }

        [ValidateInput(false)]
        public ActionResult OTGridViewPartial()
        {
            var model = unitOfWork.OvertimesRepo.Get(m => m.UserId == UserId, includeProperties: "Users,CreatedByUser,ModifiedByUser");
            return PartialView("_OTGridViewPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult OTGridViewPartialAddNew(NorthOps.Models.Overtimes item)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    item.UserId = UserId;
                    item.CreatedBy = UserId;

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
            var model = unitOfWork.OvertimesRepo.Get(m => m.UserId == UserId, includeProperties: "Users,CreatedByUser,ModifiedByUser");
            return PartialView("_OTGridViewPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult OTGridViewPartialUpdate(NorthOps.Models.Overtimes item)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    item.UserId = UserId;
                    item.ModifiedBy = UserId;
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
            var model = unitOfWork.OvertimesRepo.Get(m => m.UserId == UserId, includeProperties: "Users,CreatedByUser,ModifiedByUser");
            return PartialView("_OTGridViewPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult OTGridViewPartialDelete([ModelBinder(typeof(DevExpressEditorsBinder))]System.String Id)
        {

            if (Id != null)
            {
                try
                {
                    unitOfWork.OvertimesRepo.Delete(m => m.Id == Id);
                    unitOfWork.Save();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            var model = unitOfWork.OvertimesRepo.Get(m => m.UserId == UserId, includeProperties: "Users,CreatedByUser,ModifiedByUser");
            return PartialView("_OTGridViewPartial", model);
        }

        public ActionResult AddEditOverTimePartial([ModelBinder(typeof(DevExpressEditorsBinder))]string overTimeId)
        {
            var model = unitOfWork.OvertimesRepo.Find(m => m.Id == overTimeId);
            return PartialView("_AddEditOverTimePartial", model);
        }
    }
}