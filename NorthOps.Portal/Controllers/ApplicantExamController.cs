using Microsoft.AspNet.Identity;
using DevExpress.Web.Mvc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using NorthOps.Models;
using NorthOps.Models.Repository;
using NorthOps.Models.ViewModels;

namespace NorthOps.Portal.Controllers
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

        #region Exam Description
        public ActionResult ExamDescriptionPartial(Guid? ExamId)
        {

            return PartialView("_ExamDescriptionPartial", unitOfWork.ExamRepo.GetByID(ExamId));
        }

        #endregion

        #region List of Exam Available
        [ValidateInput(false)]
        public ActionResult ExamsGridPartial()
        {
            var userId = User.Identity.GetUserId();

            return PartialView("_ExamsGridPartial", unitOfWork.Applicant.Get(filter: m => m.UserId == userId && (m.IsTaken == null || m.IsTaken == false), includeProperties: "Exams"));
        }

        #endregion


        #region TakeExam
        [HttpPost, ValidateAntiForgeryToken]
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
        public async Task<string> TypingSpeedUpdatePartial(int? score, int? error,int? wordcounts, Guid? examId, int level = 1)
        {
            var paragraph = await unitOfWork.TypingSpeedRepo.GetAsync(filter: m => m.TypingLevel == level);
            if (score != null)
            {
               // score = (score - error);
                score = score > 50 ? 50 : score;
                var SubScore = (score * 1.0) / unitOfWork.TypingSpeedRepo.Get().Count();
                var UserId = User.Identity.GetUserId();
                var applicant = unitOfWork.Applicant.Get(filter: m => m.ExamId == examId && m.UserId == UserId).FirstOrDefault();
                applicant.Result = (applicant.Result ?? 0) + Convert.ToInt32(SubScore);
                unitOfWork.Applicant.Update(applicant);
                await unitOfWork.SaveAsync();
            }
            return paragraph.Count() == 0 ? "0" : paragraph.FirstOrDefault().Paragraph;
        }
        #endregion

        [HttpPost, ValidateInput(false)]
        public ActionResult StartExamPartial(Guid ExamId)
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
            return PartialView("_StartExamPartial", unitOfWork.ExamRepo.GetByID(ExamId));
        }

    }
}