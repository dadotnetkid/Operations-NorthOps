using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NorthOps.Models;
using NorthOps.Models.Repository;

namespace NorthOps.Ops.Controllers
{
    [Authorize(Roles ="Administrator")]
    public class ManageFormsController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        // GET: ManageForms
   
        #region Branch

        [Route("manage-form/branch")]
        public ActionResult Branch()
        {
            return View();
        }

        [ValidateInput(false)]
        public ActionResult BranchGridViewPartial()
        {
            var model = unitOfWork.BranchRepo.Get();
            return PartialView("_BranchGridViewPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult BranchGridViewPartialAddNew(NorthOps.Models.Branch item)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    unitOfWork.BranchRepo.Insert(item);
                    unitOfWork.Save();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            var model = unitOfWork.BranchRepo.Get();
            return PartialView("_BranchGridViewPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult BranchGridViewPartialUpdate(NorthOps.Models.Branch item)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    unitOfWork.BranchRepo.Update(item);
                    unitOfWork.Save();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            var model = unitOfWork.BranchRepo.Get();
            return PartialView("_BranchGridViewPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult BranchGridViewPartialDelete(Branch item)
        {


            try
            {
                unitOfWork.BranchRepo.Delete(unitOfWork.BranchRepo.Find(m => m.Id == item.Id));
                unitOfWork.Save();
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }
            var model = unitOfWork.BranchRepo.Get();
            return PartialView("_BranchGridViewPartial", model);
        }

        #endregion

        #region Division

        [Route("manage-form/division")]
        public ActionResult Division()
        {
            return View();
        }

        [ValidateInput(false)]
        public ActionResult DivisionGridViewPartial()
        {
            var model = unitOfWork.DivisionsRepo.Get();
            return PartialView("_DivisionGridViewPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DivisionGridViewPartialAddNew(NorthOps.Models.Divisions item)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    // Insert here a code to insert the new item in your model
                    unitOfWork.DivisionsRepo.Insert(item);
                    unitOfWork.Save();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            var model = unitOfWork.DivisionsRepo.Get();
            return PartialView("_DivisionGridViewPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult DivisionGridViewPartialUpdate(NorthOps.Models.Divisions item)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    // Insert here a code to update the item in your model
                    unitOfWork.DivisionsRepo.Update(item);
                    unitOfWork.Save();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            var model = unitOfWork.DivisionsRepo.Get();
            return PartialView("_DivisionGridViewPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult DivisionGridViewPartialDelete(System.Int32 Id)
        {

            if (Id >= 0)
            {
                try
                {
                    unitOfWork.DivisionsRepo.Delete(unitOfWork.DivisionsRepo.Find(m => m.Id == Id));
                    unitOfWork.Save();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            var model = unitOfWork.DivisionsRepo.Get();
            return PartialView("_DivisionGridViewPartial", model);
        }

        #endregion
        
        #region Department
        [Route("manage-form/department")]
        public ActionResult Department()
        {
            return View();
        }
        [ValidateInput(false)]
        public ActionResult DepartmentGridViewPartial()
        {
            var model = unitOfWork.DepartmentsRepo.Get();
            return PartialView("_DepartmentGridViewPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DepartmentGridViewPartialAddNew(NorthOps.Models.Departments item)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    unitOfWork.DepartmentsRepo.Insert(item);
                    unitOfWork.Save();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            var model = unitOfWork.DepartmentsRepo.Get();
            return PartialView("_DepartmentGridViewPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult DepartmentGridViewPartialUpdate(NorthOps.Models.Departments item)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    unitOfWork.DepartmentsRepo.Update(item);
                    unitOfWork.Save();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            var model = unitOfWork.DepartmentsRepo.Get();
            return PartialView("_DepartmentGridViewPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult DepartmentGridViewPartialDelete(System.Int32 Id)
        {

            if (Id >= 0)
            {
                try
                {
                    unitOfWork.DepartmentsRepo.Delete(unitOfWork.DepartmentsRepo.Find(m => m.Id == Id));
                    unitOfWork.Save();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            var model = unitOfWork.DepartmentsRepo.Get();
            return PartialView("_DepartmentGridViewPartial", model);
        }

        #endregion


    }
}