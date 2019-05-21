using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity.Owin;
using NorthOps.Models;
using NorthOps.Models.Config;
using NorthOps.Models.Repository;

namespace Northops.WebApi.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class RecruitmentApiController : ApiController
    {
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }


        UnitOfWork unitOfWork = new UnitOfWork();
        [Route("api-recruitment-applicants")]
        [HttpGet]
        public IHttpActionResult Get()
        {
            var model = unitOfWork.UserRepository.Fetch(includeProperties: "UserRoles,Applicants")
             .Where(x => x.UserRoles.Any(u => u.Name == "Applicant") && !x.Applicants.Any()).Select(x => new
             {

                 Id = x.Id,
                 FirstName = x.FirstName,
                 MiddleName = x.MiddleName,
                 LastName = x.LastName,
                 Email = x.Email,
                 CreatedDate = x.CreatedDate,
                 EmailConfirmed = x.EmailConfirmed ? "Confirmed" : "Not Confirmed"
             });
            return Ok(model);
        }

        [HttpPost]
        [Route("api-open-exam")]
        public async Task<IHttpActionResult> OpenExam()
        {

            string userId = HttpContext.Current.Request.Params["userId"];
            if (!unitOfWork.UserRepository.Fetch().Any(m => m.Id == userId))
            {
                return Get();
            }

            var exams = unitOfWork.ExamRepo.Get();
            foreach (var i in exams)
            {
                unitOfWork.Applicant.Insert(new Applicants()
                {
                    ApplicantId = Guid.NewGuid(),
                    ExamId = i.ExamId,
                    UserId = userId

                });
                unitOfWork.Save();
            }
            await UserManager.SendEmailAsync(userId, "Your exam is ready", $"Your exam is ready. Please log in to your NorthOps account and click the exam button to start your online exam.<br/> or click on the link below to proceed to your log in page. <br/> <a href='http://portal.northops.asia/applicant-exam/index'>click here</a>");
            return Get();
        }
    }
}
