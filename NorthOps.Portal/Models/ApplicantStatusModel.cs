using NorthOps.Portal.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
namespace NorthOps.Portal.Models
{
    public partial class ApplicantStatusModel
    {
        public bool? Done { get; set; }
        public string Field { get; set; }
        public string Status { get; set; }
        UnitOfWork unitOfWork = new UnitOfWork();
        public string UserId { get { return HttpContext.Current.User.Identity.GetUserId(); } }
        public IEnumerable<ApplicantStatusModel> applicantStatusModel()
        {
            List<ApplicantStatusModel> applicantStatus = new List<ApplicantStatusModel>();
            //bool? Resume = unitOfWork.UserRepository.Get(filter: m => m.FirstName != null && m.Id == UserId && m.EmailConfirmed == true).Count() > 0 ? true : false;
            foreach (var i in unitOfWork.UserRepository.Get(filter: m => m.Id == UserId))
            {
                if (i.FirstName == null)
                {
                    applicantStatus.Add(new ApplicantStatusModel() { Field = "Resume", Done = false, Status = "Update your Profile" });
                }
                else if (!i.EmailConfirmed)
                {
                    applicantStatus.Add(new ApplicantStatusModel() { Field = "Resume", Done = false, Status = "Check your email for verification" });
                }
                else
                {
                    applicantStatus.Add(new ApplicantStatusModel() { Field = "Resume", Done = true });
                }

            }



            foreach (var i in unitOfWork.JobApplicationRepo.Get(filter: m => m.UserId == this.UserId))
            {


                applicantStatus.Add(new ApplicantStatusModel()
                {
                    Field = "Exam",
                    Done = i.Exam,
                    Status = unitOfWork.Applicant.Get(filter: m => m.UserId == this.UserId).Count() > 0 ? "Your exam is ready" : string.Empty
                });
                applicantStatus.Add(new ApplicantStatusModel()
                {
                    Field = "Phone Interview",
                    Done = i.PhoneInterview,
                    Status = i.PhoneInterviewDate.ToString()
                });
                applicantStatus.Add(new ApplicantStatusModel()
                {
                    Field = "Personal Interview",
                    Status = i.PersonalInterviewDate.ToString(),
                    Done = i.PersonalInterview
                });
                applicantStatus.Add(new ApplicantStatusModel()
                {
                    Field = "Training",
                    Status = i.TrainingDate.ToString(),
                    Done = i.Training
                });
                applicantStatus.Add(new ApplicantStatusModel()
                {
                    Field = "On Boarding",
                    Done = i.OnBoarding,
                    Status = i.OnBoardingDate.ToString()
                });
                applicantStatus.Add(new ApplicantStatusModel()
                {
                    Field = "Contact",
                    Done = i.Contract,
                    Status = i.ContractDate.ToString()
                });
            }
            return applicantStatus;

        }

    }
    public partial class ApplicantStatusModel
    {

    }
}