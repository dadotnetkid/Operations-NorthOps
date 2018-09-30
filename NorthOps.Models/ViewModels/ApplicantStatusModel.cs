using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.AspNet.Identity;
using NorthOps.Models.Repository;

namespace NorthOps.Models
{
    public partial class ApplicantStatusModel
    {
        private RequestContext requestContext;
        public ApplicantStatusModel()
        {
            this.requestContext = HttpContext.Current.Request.RequestContext;
        }
        public bool? Done { get; set; }
        public string Field { get; set; }
        public string Status { get; set; }
        UnitOfWork unitOfWork = new UnitOfWork();

        public string UserId
        {
            get
            {
                return HttpContext.Current.User.Identity.GetUserId();
            }

        }

        public IEnumerable<ApplicantStatusModel> applicantStatusModel(string UserId = "")
        {
            var applicantStatus = new List<ApplicantStatusModel>();
            var _userId = string.IsNullOrEmpty(UserId) ? this.UserId : UserId;
            var user = unitOfWork.UserRepository.Find(filter: m => m.Id == _userId);
            var status = unitOfWork.Applicant.Get(filter: m => m.UserId == _userId).Any() ? $"Your exam is ready <a href='{new UrlHelper(requestContext).Action("Index", "ApplicantExam")  }'> click here</a>" : string.Empty;
            applicantStatus.Add(new ApplicantStatusModel()
            {
                Field = "Resume",
                Done = user?.FirstName != null ? true : false,
                Status = user?.FirstName != null ? user?.EmailConfirmed == true ? "Thank you for registering. We will be loading your online exam soon.  Please log in again within the day to check if your exam has been assigned." : "Please check your email for verification" : $"Please complete your profile information <a href='{new UrlHelper(requestContext).Action("Profile", "Member")}'>here</a>"

            });

            foreach (var i in unitOfWork.JobApplicationRepo.Get(filter: m => m.UserId == _userId, includeProperties: "Users"))
            {
                var examResult = $"Congratulations {i.Users.FullName} You passed the online exam for NorthOps. We are located at Josephine Hotel formerly Sports City Bayombong. Kindly look for Alvin and bring a copy of your resume. Thank you.";

                if (i.Users?.Applicants?.Any(m => m.IsTaken == null) == true)
                {
                    examResult =
                        $"Your exam is ready <a href='{new UrlHelper(requestContext).Action("Index", "ApplicantExam")}'> click here</a>";
                }
                else if (i.IsExamPassed == false)
                {
                    examResult = "Sorry to inform you that you've failed the preliminary examination.";
                }
                else if (i.Users?.Applicants?.Any(m => m.IsTaken == null) == false)
                {
                    examResult = "Wait for notification";
                }
                else if (i.Users?.Applicants?.Any() == false)
                {
                    //examResult = unitOfWork.Applicant.Get(filter: m => m.UserId == _userId).Any() ? $"Your exam is ready <a href='{new UrlHelper(requestContext).Action("Index", "ApplicantExam")}'> click here</a>" : string.Empty;
                    examResult = string.Empty;
                }
                else
                {
                    examResult = string.Empty;
                }

                applicantStatus.Add(new ApplicantStatusModel()
                {
                    Field = "Exam",
                    Done = i.Exam,
                    Status = examResult


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
                    Status = i.Contract.ToString()
                });
            }
            return applicantStatus;

        }

    }
    public partial class ApplicantStatusModel
    {

    }
}