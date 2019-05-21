using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Northops.WebApi.Models;
using NorthOps.Models;
using NorthOps.Models.Repository;
using NorthOps.Models.ViewModels;

namespace Northops.WebApi.Controllers
{
    [Authorize]
    public class ExamApiController : ApiController
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        private string UserId => HttpContext.Current.User.Identity.GetUserId();

        [HttpPost]
        [Route("api-exam-list")]
        public IHttpActionResult Exams()
        {
            var examId = Guid.Parse("9B6BD719-6596-455C-9568-0E3B8775324B");
            var applicants = unitOfWork.Applicant.Fetch(m => (m.UserId == UserId && m.IsTaken == null) && m.Exams.CategoryId != examId);
            return Ok(applicants.Select(x => new { x.ExamId, x.Exams.ExamName, x.Exams.ExamType, x.Exams.Duration, x.Exams.Items }).ToList().Select(x => new { x.ExamId, x.ExamName, x.Items, x.Duration, ExamType = new Exams().GetExamType(x.ExamType) }));
        }
        [HttpPost]
        [Route("api-start-exam")]
        public IHttpActionResult StartExam()
        {
            Guid examId = Guid.NewGuid();
            Guid.TryParse(HttpContext.Current.Request.Params["ExamId"], out examId);

            var applicant = unitOfWork.Applicant.Get(filter: m => m.UserId == UserId && m.ExamId == examId).FirstOrDefault();
            if (applicant != null)
            {
                applicant.IsTaken = true;
                applicant.DateTimeTaken = DateTime.Now;
                unitOfWork.Applicant.Update(applicant);
                unitOfWork.Save();
            }
            return Ok();
        }

        [HttpPost]
        [Route("api-questions")]
        public IHttpActionResult Questions()
        {
            Guid examId = Guid.NewGuid();
            Guid.TryParse(HttpContext.Current.Request.Params["ExamId"], out examId);
            var questions = unitOfWork.QuestionRepo.Fetch(m => m.ExamId == examId);
            Exams exams = unitOfWork.ExamRepo.Find(m => m.ExamId == examId);
            var model = questions.Select(x => new
            {
                x.QuestionId,
                x.Question,
                x.Number,
                x.Exams.Duration,
                x.Exams.Items,
                Choices = x.Choices.Select(c => new { c.Choice, c.ChoiceId, c.ChoiceLetter })
            });
            if (exams.Categories.CategoryName == "Behavioral")
            {
                return Ok(model.OrderBy(m => m.Number));
            }

            return Ok(model
                .OrderBy(x => Guid.NewGuid()).Skip(0).Take(50).ToList());
        }

        [HttpPost]
        [Route("api-submit-answer")]
        public async System.Threading.Tasks.Task<IHttpActionResult> SubmitAnswerAsync([FromBody]ExamViewModel item)
        {
            return Ok();

            ApplicantExamModel applicantExam = new ApplicantExamModel();
            await applicantExam.TakeExam(new Questions() { QuestionId = item.QuestionId },
                new Choices() { ChoiceId = item.AnswerId });

            return Ok();
        }
    }
}
