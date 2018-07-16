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
            var model = unitOfWork.UserRepository.Get();
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
    }
}