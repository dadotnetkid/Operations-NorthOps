using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NorthOps.Models.Repository;

namespace NorthOps.Ops.Controllers
{
    public class HolidayController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork(); 
        public ActionResult Index()
        {
            return View();
        }

        [ValidateInput(false)]
        public ActionResult HolidayGridViewPartial()
        {
            var model = unitOfWork.HolidaysRepo.Get();
            return PartialView("_HolidayGridViewPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult HolidayGridViewPartialAddNew(NorthOps.Models.Holidays item)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    unitOfWork.HolidaysRepo.Insert(item);
                    unitOfWork.Save();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            var model = unitOfWork.HolidaysRepo.Get();
            return PartialView("_HolidayGridViewPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult HolidayGridViewPartialUpdate(NorthOps.Models.Holidays item)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    unitOfWork.HolidaysRepo.Update(item);
                    unitOfWork.Save();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            var model = unitOfWork.HolidaysRepo.Get();
            return PartialView("_HolidayGridViewPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult HolidayGridViewPartialDelete([ModelBinder(typeof(DevExpressEditorsBinder))]int Id)
        {
            
            if (Id >= 0)
            {
                try
                {
                    unitOfWork.HolidaysRepo.Delete(m => m.Id == Id);
                    unitOfWork.Save();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            var model = unitOfWork.HolidaysRepo.Get();
            return PartialView("_HolidayGridViewPartial", model);
        }

        public ActionResult AddEditHolidayPartial([ModelBinder(typeof(DevExpressEditorsBinder))]
            int? HolidayId)
        {
            return PartialView("_AddEditHolidayPartial",unitOfWork.HolidaysRepo.Find(m=>m.Id==HolidayId));
        }
    }
}