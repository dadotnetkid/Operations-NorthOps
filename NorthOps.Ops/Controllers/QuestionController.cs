using DevExpress.Web.Mvc;
using NorthOps.Ops.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NorthOps.Models;
using NorthOps.Models.Repository;

namespace NorthOps.Ops.Controllers
{
    public class QuestionController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        public ActionResult Index()
        {
            return View();
        }

        [ValidateInput(false)]
        public ActionResult QuestionGridPartial()
        {
            var model = new object[0];
            return PartialView("_QuestionGridPartial", unitOfWork.QuestionRepo.Get(includeProperties: "Exam"));
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult QuestionGridPartialAddNew(Questions item)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    unitOfWork.QuestionRepo.Insert(item);
                    unitOfWork.Save();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("_QuestionGridPartial", unitOfWork.QuestionRepo.Get(includeProperties: "Exam"));
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult QuestionGridPartialUpdate(Questions item)
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
            return PartialView("_QuestionGridPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult QuestionGridPartialDelete(System.Guid? QuestionId)
        {
            var model = new object[0];
            if (QuestionId != null)
            {
                try
                {
                    var questionRepo = unitOfWork.QuestionRepo.Find(m => m.QuestionId == QuestionId);
                    unitOfWork.QuestionRepo.Delete(questionRepo);
                    unitOfWork.Save();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            return PartialView("_QuestionGridPartial", unitOfWork.QuestionRepo.Get(includeProperties: "Exam"));
        }

        [ValidateInput(false)]
        public ActionResult ChoicesGridPartial(Guid QuestionId, System.Guid? ChoiceId)
        {
            var model = unitOfWork.ChoiceRepo.Get(filter: m => m.QuestionId == QuestionId, includeProperties: "Question");
            ViewBag.QuestionId = QuestionId;
            if (ChoiceId != null)
            {
                model = unitOfWork.ChoiceRepo.Get(filter: m => m.ChoiceId == ChoiceId, includeProperties: "Question");
            }

            return PartialView("_ChoicesGridPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ChoicesGridPartialAddNew(Choices item)
        {
            var model = new object[0];
            if (ModelState.IsValid)
            {
                try
                {
                    item.ChoiceId = Guid.NewGuid();
                    item.DateCreated = DateTime.Now;
                    unitOfWork.ChoiceRepo.Insert(item);
                    unitOfWork.Save();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            ViewBag.QuestionId = item.QuestionId;
            return PartialView("_ChoicesGridPartial", unitOfWork.ChoiceRepo.Get(filter: m => m.QuestionId == item.QuestionId, includeProperties: "Question"));
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult ChoicesGridPartialUpdate(Choices item, Guid? ChoiceId)
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
            return PartialView("_ChoicesGridPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult ChoicesGridPartialDelete(System.Guid ChoiceId, System.Guid QuestionId)
        {
            var model = new object[0];
            ViewBag.QuestionId = QuestionId;
            if (ChoiceId != null)
            {
                try
                {

                    var choice = unitOfWork.ChoiceRepo.GetByID(ChoiceId);
                    unitOfWork.ChoiceRepo.Delete(choice);
                    unitOfWork.Save();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            return PartialView("_ChoicesGridPartial", unitOfWork.ChoiceRepo.Get(filter: m => m.QuestionId == QuestionId, includeProperties: "Question"));
        }
    }
}