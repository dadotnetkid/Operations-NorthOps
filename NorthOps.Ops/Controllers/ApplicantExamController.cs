using Microsoft.AspNet.Identity;
using DevExpress.Web.Mvc;
using NorthOps.Ops.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using NorthOps.Models;
using NorthOps.Models.Repository;
using NorthOps.Models.ViewModels;


namespace NorthOps.Ops.Controllers
{
    [RoutePrefix("applicant-exam")]
    [Authorize(Roles = "Applicant,Administrator")]
    public class ApplicantExamController : Controller
    {
        UnitOfWork unitOfWork = new UnitOfWork();
        [Route("index")]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult TakeExam(Guid ExamId)
        {
            var userId = User.Identity.GetUserId();
            var applicant = unitOfWork.Applicant.Get(filter: m => m.UserId == userId && m.ExamId == ExamId).FirstOrDefault();
            if (applicant != null)
            {
                applicant.IsTaken = true;
                applicant.DateTimeTaken = DateTime.Now;
                unitOfWork.Applicant.Update(applicant);
                unitOfWork.Save();
            }
            return PartialView("_TakeExamPartial", unitOfWork.ExamRepo.GetByID(ExamId));
        }

        public ActionResult ExamDescriptionPartial(Guid? ExamId)
        {
            //unitOfWork.Applicant.Insert(new Applicant() {ApplicantId=Guid.NewGuid(),ExamId=ExamId, UserId=User.Identity.GetUserId(),DateTimeTaken=DateTime.Now  });
            // unitOfWork.Save();
            return PartialView("_ExamDescriptionPartial", unitOfWork.ExamRepo.GetByID(ExamId));
        }


        [ValidateInput(false)]
        public ActionResult ExamsGridPartial()
        {
            var model = new object[0];
            var userId = User.Identity.GetUserId();

            return PartialView("_ExamsGridPartial", unitOfWork.Applicant.Get(filter: m => m.UserId == userId && (m.IsTaken == null || m.IsTaken == false), includeProperties: "Exam"));
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ExamsGridPartialAddNew(Exams item)
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
            return PartialView("_ExamsGridPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult ExamsGridPartialUpdate(Exams item)
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
            return PartialView("_ExamsGridPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult ExamsGridPartialDelete(System.Guid ExamId)
        {
            var model = new object[0];
            if (ExamId != null)
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
            return PartialView("_ExamsGridPartial", model);
        }
        #region TakeExam
        [HttpPost]
        public async Task<int> TakeExamPartialUpdate(Questions question, Choices choice)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await new ApplicantExamModel().TakeExam(question, choice);


                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            return 0;
        }
        public ActionResult TypingSpeedPartial()
        {
            var model = unitOfWork.TypingSpeedRepo.Get().FirstOrDefault();
            return PartialView("_typingspeed", model);
        }
        public async Task<string> TypingSpeedUpdatePartial(int? Score, int? Error, Guid? ExamId, int Level = 1)
        {
            var paragraph = await unitOfWork.TypingSpeedRepo.GetAsync(filter: m => m.TypingLevel == Level);
            if (Score != null)
            {
                Score = Score > 50 ? 50 : Score;
                Score = Score / paragraph.Count();
                var UserId = User.Identity.GetUserId();
                var applicant = unitOfWork.Applicant.Get(filter: m => m.ExamId == ExamId && m.UserId == UserId).FirstOrDefault();
                applicant.Result = (applicant.Result ?? 0) + Score;
                unitOfWork.Applicant.Update(applicant);
                await unitOfWork.SaveAsync();
            }
            return paragraph.Count() == 0 ? "finish" : paragraph.FirstOrDefault().Paragraph;
        }
        #endregion

    }
}