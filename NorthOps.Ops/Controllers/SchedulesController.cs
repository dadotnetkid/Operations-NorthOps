using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NorthOps.Models.Repository;

namespace NorthOps.Ops.Controllers
{
    public class SchedulesController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();

        #region Partials
        public ActionResult cboEmployeesPartial()
        {
            var model = unitOfWork.UserRepository.Fetch().Where(m => m.UserRoles.Any(x => x.Name == "Employee")).ToList();
            return PartialView("_cboEmployeesPartial", model);
        }


        #endregion
        #region Schedules
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SchedulerPartial(string UserId = "")
        {
            var appointments = unitOfWork.SchedulesRepo.Get(includeProperties: "Users", filter: m => m.UserId.Contains(UserId));
            ViewData["Appointments"] = appointments;
            //iewData["Resources"] = resources;
            ViewBag.UserId = UserId;
            return PartialView("_SchedulerPartial", appointments);
        }


        public ActionResult GenerateSchedules()
        {
            return View();
        }
        #endregion

        #region Gridview
        [ValidateInput(false)]
        public ActionResult SchedulesGridViewPartial()
        {
            var model = unitOfWork.SchedulesRepo.Get(includeProperties: "Users");
            return PartialView("_SchedulesGridViewPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SchedulesGridViewPartialAddNew(NorthOps.Models.Schedules item)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    item.Id = Guid.NewGuid().ToString();
                    
                    unitOfWork.SchedulesRepo.Insert(item);
                    unitOfWork.Save();
                    // Insert here a code to insert the new item in your model
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            var model = unitOfWork.SchedulesRepo.Get(includeProperties: "Users");
            return PartialView("_SchedulesGridViewPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult SchedulesGridViewPartialUpdate(NorthOps.Models.Schedules item)
        {

            if (ModelState.IsValid)
            {
                try
                {
                   
                    unitOfWork.SchedulesRepo.Update(item);
                    unitOfWork.Save();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            var model = unitOfWork.SchedulesRepo.Get(includeProperties:"Users");
            return PartialView("_SchedulesGridViewPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult SchedulesGridViewPartialDelete([ModelBinder(typeof(DevExpressEditorsBinder))]System.String Id)
        {
         
            if (Id != null)
            {
                try
                {
                    unitOfWork.SchedulesRepo.Delete(m => m.Id == Id);
                    unitOfWork.Save();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            var model = unitOfWork.SchedulesRepo.Get(includeProperties: "Users");
            return PartialView("_SchedulesGridViewPartial", model);
        }

        public ActionResult AddEditSchedulePartial([ModelBinder(typeof(DevExpressEditorsBinder))]System.String scheduleId)
        {
            var model = unitOfWork.SchedulesRepo.Find(m => m.Id == scheduleId);
            return PartialView("_AddEditSchedulePartial", model);
        }
        #endregion
    }
}