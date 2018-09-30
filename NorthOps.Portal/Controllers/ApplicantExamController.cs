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
    [Authorize(Roles = "Applicant,Administrator")]
    [RoutePrefix("applicant-exam")]
    public class ApplicantExamController : Controller
    {
        UnitOfWork unitOfWork = new UnitOfWork();
        [Route("index/{ExamType?}")]
        public ActionResult Index(int ExamTypes = 0)
        {
            ViewBag.ExamTypes = ExamTypes;
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
        public ActionResult ExamsGridPartial(int ExamTypes = 0)
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
        public async Task<string> TypingSpeedUpdatePartial(int? score, int? error, int? wordcounts, Guid? examId, int level = 1)
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

        public ActionResult AudioPartial([ModelBinder(typeof(DevExpressEditorsBinder))]Guid AudioId)
        {

            return PartialView("_AudioPartial", new Videos() { VideoId = AudioId });
        }
        [HttpPost]
        public ActionResult IdentificationExamPartial([ModelBinder(typeof(DevExpressEditorsBinder))] IdentificationExamViewModel vm)
        {
            var Score = 0;
            try
            {
                
                var userId = User.Identity.GetUserId();

                var item = unitOfWork.QuestionRepo.Find(m => m.QuestionId == vm.QuestionId);
                var applicants = unitOfWork.Applicant.Find(m => m.UserId == userId && (ExamTypes)m.Exams.ExamType == ExamTypes.Identification);

                if (item.Choices.Any(m => m.Choice == vm.Choice.ToUpper()))
                {

                    if (applicants == null)
                    {
                        unitOfWork.Applicant.Insert(new Applicants()
                        {
                            ApplicantId = Guid.NewGuid(),
                            ExamId = item.ExamId,
                            DateTimeTaken = DateTime.Now,
                            Result = 1,
                            UserId = userId,
                            IsTaken = true
                        });
                    }
                    else
                    {
                        if (applicants.Result == null)
                            applicants.Result = 0;
                        applicants.Result++;
                    }


                    unitOfWork.Save();
                }
                else
                {
                    vm.CorrectAnswer = item.Choices?.FirstOrDefault()?.Choice;
                }

                Score = applicants.Result ?? 0;
                unitOfWork.ApplicantAnswer.Insert(new ApplicantAnswers()
                {
                    ApplicantAnswerId = Guid.NewGuid(),
                    QuestionId = item.QuestionId,
                    SessionId = vm.SessionId,
                    UserId = User.Identity.GetUserId()

                });
                unitOfWork.Save();
           
            }

            catch (Exception e)
            {

            }
            vm.Item++;
            vm.Score = Score;
            List<Guid?> question = unitOfWork.ApplicantAnswer.Fetch(m => m.SessionId == vm.SessionId).Select(x => x.QuestionId).ToList();
            vm.Questions = new UnitOfWork().QuestionRepo.Fetch()
                   .OrderBy(m => Guid.NewGuid()).Where(m => !question.Contains(m.QuestionId)).FirstOrDefault(m => (ExamTypes)m.Exams.ExamType == ExamTypes.Identification);


            return PartialView("_IdentificationExamPartial", vm);
        }

        public ActionResult IdentificationExamPartial()
        {
            IdentificationExamViewModel vm = new IdentificationExamViewModel();
            var userId = User.Identity.GetUserId();

            vm.Questions = new UnitOfWork().QuestionRepo.Fetch()
                .OrderBy(m => Guid.NewGuid()).FirstOrDefault(m => (ExamTypes)m.Exams.ExamType == ExamTypes.Identification);
            vm.SessionId = Guid.NewGuid().ToString();
            vm.Item = 1;
            var applicants = unitOfWork.Applicant.Find(m => m.UserId == userId && (ExamTypes)m.Exams.ExamType == ExamTypes.Identification);
            if (applicants != null)
            {
                applicants.Result = 0;
                unitOfWork.Save();
            }


            return PartialView("_IdentificationExamPartial", vm);
        }
    }
}