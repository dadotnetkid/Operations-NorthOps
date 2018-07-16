using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using NorthOps.Models;
using NorthOps.Models.Repository;

namespace NorthOps.Portal.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        private string _userId;

        public string UserId
        {
            get
            {

                return User.Identity.GetUserId(); 

            }
     
        }

        public ActionResult Index()
        {
            
            ViewBag.Notification = unitOfWork.EmployeeNoticationsRepo.Get(m=>m.Id==UserId);
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Index(string str = "")
        {
          for(var i=1;i<=70;i++)
            {
                Debug.WriteLine($"{i}:{i % 7}");
            }
            return View();
        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [ValidateInput(false)]
        public ActionResult ApplicationStatusGridViewPartial()
        {
            var model = new ApplicantStatusModel().applicantStatusModel();
            return PartialView("_ApplicationStatusGridViewPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ApplicationStatusGridViewPartialAddNew(ApplicantStatusModel item)
        {
            var model = new object[0];
            if (ModelState.IsValid)
            {
                try
                {
                    // Insert here a code to insert the new item in your model
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("_ApplicationStatusGridViewPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult ApplicationStatusGridViewPartialUpdate(ApplicantStatusModel item)
        {
            var model = new object[0];
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
            return PartialView("_ApplicationStatusGridViewPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult ApplicationStatusGridViewPartialDelete(System.String UserId)
        {
            var model = new object[0];
            if (UserId != null)
            {
                try
                {
                    // Insert here a code to delete the item from your model
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            return PartialView("_ApplicationStatusGridViewPartial", model);
        }
    }
}