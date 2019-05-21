using System;
using System.Linq;
using System.Web.Mvc;
using DevExpress.Web.Mvc;
using Microsoft.AspNet.Identity;
using NorthOps.Models.Repository;

namespace NorthOps.Portal.Controllers
{
    [Authorize]
    public class LeavesController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        private string UserId => User.Identity.GetUserId();
        [Route("employees/leaves")]
        public ActionResult Index()
        {
            return View();
        }

        [ValidateInput(false)]
        public ActionResult LeavesGridViewPartial()
        {
            var model = unitOfWork.LeavesRepo.Fetch(includeProperties: "Users,LeaveTypes").Where(m=>m.UserId==UserId).ToList();
            
            return PartialView("_LeavesGridViewPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult LeavesGridViewPartialAddNew([ModelBinder(typeof(DevExpressEditorsBinder))]NorthOps.Models.Leaves item)
        {
            //     var model = new object[0];
            if (ModelState.IsValid)
            {
                try
                {
                    
                    
                    item.DateCreated = DateTime.Now;
                    unitOfWork.LeavesRepo.Insert(item);
                    unitOfWork.Save();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            var model = unitOfWork.LeavesRepo.Fetch(includeProperties: "Users,LeaveTypes").Where(m => m.UserId == UserId).ToList();
            return PartialView("_LeavesGridViewPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult LeavesGridViewPartialUpdate(NorthOps.Models.Leaves item)
        {
            //var model = new object[0];
            if (ModelState.IsValid)
            {
                try
                {
                    var leaves = unitOfWork.LeavesRepo.Find(m => m.Id == item.Id);

                    leaves.ModifiedBy = User.Identity.GetUserId();
                    leaves.UserId = item.UserId;
                    leaves.DateFrom = item.DateFrom;
                    leaves.DateTo = item.DateTo;
                    //leaves.isAdminApproved = Convert.ToBoolean(item.isAdminApproved != null);
                    if (leaves.isAdminApproved == null)
                    {
                        unitOfWork.Save();
                    }
                    else
                    {
                        ViewData["EditError"] = "System cant delete this request. Management already modified your request";
                    }
                    
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            var model = unitOfWork.LeavesRepo.Fetch(includeProperties: "Users,LeaveTypes").Where(m => m.UserId == UserId).ToList();
            return PartialView("_LeavesGridViewPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult LeavesGridViewPartialDelete([ModelBinder(typeof(DevExpressEditorsBinder))]System.Int32 Id)
        {
            var leaves = unitOfWork.LeavesRepo.Find(m => m.Id == Id);
            if (leaves.isAdminApproved == null)
            {
                if (Id >= 0)
                {
                    try
                    {
                        unitOfWork.LeavesRepo.Delete(m => m.Id == Id);
                        unitOfWork.Save();
                    }
                    catch (Exception e)
                    {
                        ViewData["EditError"] = e.Message;
                    }
                }
            }
            else
            {
                ViewData["EditError"] = "System cant delete this request. Management already modified your request";
            }

          
            var model = unitOfWork.LeavesRepo.Fetch(includeProperties: "Users,LeaveTypes").Where(m => m.UserId == UserId).ToList();
            return PartialView("_LeavesGridViewPartial", model);
        }


        public ActionResult cboEmployeePartial(string userId)
        {
            ViewBag.UserId = userId;
            var model = unitOfWork.UserRepository.Get(m => m.UserRoles.Any(x => x.Name == "Employee"));
            return PartialView("_cboEmployeePartial", model);
        }

        public ActionResult AddEditLeavesGridViewPartial([ModelBinder(typeof(DevExpressEditorsBinder))] int? Id)
        {
            var model = unitOfWork.LeavesRepo.Find(m => m.Id == Id);
            return PartialView("_AddEditLeavesGridViewPartial", model);
        }
    }
}