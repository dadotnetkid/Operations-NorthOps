using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using NorthOps.Models.Repository;
using NorthOps.Ops.Repository;

namespace NorthOps.Ops.Controllers
{
    [RoutePrefix("file-management")]
    public class FileManagementDataController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        #region Profile Data
        public ActionResult ProfileData()
        {
            return View();
        }

        [ValidateInput(false)]
        public ActionResult FilemanagementGridViewPartial()
        {
            var model = unitOfWork.FileManagementDataRepo.Get();

            return PartialView("_FilemanagementGridViewPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> FilemanagementGridViewPartialAddNew(NorthOps.Ops.Models.FileManagementData item)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    item.FileManagementId = Guid.NewGuid().ToString();
                    item.DateCreated = DateTime.Now;
                    unitOfWork.FileManagementDataRepo.Insert(item);
                    await unitOfWork.SaveAsync();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            var model = unitOfWork.FileManagementDataRepo.Get();
            return PartialView("_FilemanagementGridViewPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult FilemanagementGridViewPartialUpdate(NorthOps.Ops.Models.FileManagementData item)
        {

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
            var model = unitOfWork.FileManagementDataRepo.Get();
            return PartialView("_FilemanagementGridViewPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> FilemanagementGridViewPartialDelete(System.String FileManagementId)
        {
        
            if (FileManagementId != null)
            {
                try
                {
                    await unitOfWork.FileManagementDataRepo.DeleteAsync(m => m.FileManagementId == FileManagementId);
                   
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            var model = unitOfWork.FileManagementDataRepo.Get();
            return PartialView("_FilemanagementGridViewPartial", model);
        }
        #endregion

    }
}