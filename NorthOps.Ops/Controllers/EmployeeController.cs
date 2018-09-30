using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using NorthOps.AspIdentity;
using NorthOps.Models;
using NorthOps.Models.Repository;
using NorthOps.Services.Helpers;

namespace NorthOps.Ops.Controllers
{

    public class EmployeeController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        private ApplicationUserManager _userManager;

        #region Partials

        public ActionResult cboEmployeePartial(string userId = "")
        {

            ViewBag.UserId = userId;
            var model = unitOfWork.UserRepository.Get();
            return PartialView("_cboEmployeePartial", model);
        }

        public ActionResult TokenUserRolesPartial(string UserId)
        {
            ViewBag.UserRoles = unitOfWork.UserRepository.Find(m => m.Id == UserId)?.UserRoles;
            return PartialView("_TokenUserRolesPartial", unitOfWork.RoleRepository.Get());
        }

        #endregion
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        // GET: Employee
        public ActionResult Index()
        {
            var model = unitOfWork.UserRepository.Get(filter: m => m.UserRoles.Any(e => e.Name.Contains("employee")));
            if (Session["EmployeeModel"] == null)
                Session["EmployeeModel"] = model;
            return View(model);
        }

        [ValidateInput(false)]
        public ActionResult EmployeesGridViewPartial()
        {
            var model = unitOfWork.UserRepository.Get(filter: m => m.UserRoles.Any(e => e.Name.Contains("employee")));
            return PartialView("_EmployeesGridViewPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> EmployeesGridViewPartialAddNew(NorthOps.Models.Users item)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    //  var res = Request.Params["UserRole"];
                    await UserManager.RemoveFromRoleAsync(item.Id, "Applicant");
                    await UserManager.AddToRoleAsync(item.Id, item.userRole);
                    var res = unitOfWork.UserRepository.Find(m => m.Id == item.Id);
                    res.Position = item.Position ?? res.Position;
                    res.DivisionId = item.DivisionId ?? res.DivisionId;
                    res.DepartmentId = item.DepartmentId ?? res.DepartmentId;
                    res.BranchId = item.BranchId ?? res.BranchId;
                    if (item._BiometricId != null)
                        res.Biometrics.Add(new Biometrics() { UserId = res.Id, BiometricId = item._BiometricId });
                    unitOfWork.Save();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            var model = unitOfWork.UserRepository.Get(filter: m => m.UserRoles.Any(e => e.Name.Contains("employee")));
            Session["EmployeeModel"] = model;
            return PartialView("_EmployeesGridViewPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> EmployeesGridViewPartialUpdate(NorthOps.Models.Users item)
        {

            if (ModelState.IsValid)
            {
                try
                {

                    await UserManager.RemoveFromRolesAsync(item.Id, (await UserManager.GetRolesAsync(item.Id)).ToArray());
                    await UserManager.AddToRoleAsync(item.Id, item.userRole);
                    var res = unitOfWork.UserRepository.Find(m => m.Id == item.Id);
                    res.Position = item.Position ?? res.Position;
                    res.DivisionId = item.DivisionId ?? res.DivisionId;
                    res.DepartmentId = item.DepartmentId ?? res.DepartmentId;
                    res.BranchId = item.BranchId ?? res.BranchId;

                    if (res.Biometrics.Any())
                    {
                        unitOfWork.BiometricsRepo.Delete(unitOfWork.BiometricsRepo.Find(
                            m => (m.UserId == item.Id && m.BiometricId < 5000)));
                    }
                    res.Biometrics.Add(new Biometrics() { UserId = res.Id, BiometricId = item._BiometricId });

                    unitOfWork.Save();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            var model = unitOfWork.UserRepository.Get(filter: m => m.UserRoles.Any(e => e.Name.Contains("employee")));
            Session["EmployeeModel"] = model;
            return PartialView("_EmployeesGridViewPartial", Session["EmployeeModel"]);
        }
        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> EmployeesGridViewPartialDelete([ModelBinder(typeof(DevExpressEditorsBinder))]NorthOps.Models.Users item)
        {
            //var model = new object[0];

            try
            {
                await UserManager.RemoveFromRolesAsync(item.Id, (await UserManager.GetRolesAsync(item.Id)).ToArray());
                await UserManager.AddToRoleAsync(item.Id, "Applicant");
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }

            var model = unitOfWork.UserRepository.Get(filter: m => m.UserRoles.Any(e => e.Name.Contains("employee")));
            Session["EmployeeModel"] = model;
            return PartialView("_EmployeesGridViewPartial", Session["EmployeeModel"]);
        }

        public ActionResult AddEditCallbackEmployeePartial([ModelBinder(typeof(DevExpressEditorsBinder))] Users item)
        {
            var model = unitOfWork.UserRepository.Find(m => m.Id == item.Id);

            return PartialView("_AddEditCallbackEmployeePartial", model);
        }
        public ActionResult AddEditEmployeePartial([ModelBinder(typeof(DevExpressEditorsBinder))] string userId)
        {
            var model = unitOfWork.UserRepository.Find(m => m.Id == userId);
            return PartialView("_AddEditEmployeePartial", model);
        }

        public ActionResult ExportTo(string OutputFormat)
        {
            var model = Session["EmployeeModel"];

            switch (OutputFormat.ToUpper())
            {
                case "CSV":
                    return GridViewExtension.ExportToCsv(EmployeesGridViewHelper.ExportGridViewSettings, model);
                case "PDF":
                    return GridViewExtension.ExportToPdf(EmployeesGridViewHelper.ExportGridViewSettings, model);
                case "RTF":
                    return GridViewExtension.ExportToRtf(EmployeesGridViewHelper.ExportGridViewSettings, model);
                case "XLS":
                    return GridViewExtension.ExportToXls(EmployeesGridViewHelper.ExportGridViewSettings, model);
                case "XLSX":
                    return GridViewExtension.ExportToXlsx(EmployeesGridViewHelper.ExportGridViewSettings, model);
                default:
                    return RedirectToAction("Index");
            }
        }

    }
}
