﻿using System;
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
            this. requestContext=HttpContext.Current.Request.RequestContext;
        }
        public bool? Done { get; set; }
        public string Field { get; set; }
        public string Status { get; set; }
        UnitOfWork unitOfWork = new UnitOfWork();

        public string UserId => HttpContext.Current.User.Identity.GetUserId();

        public IEnumerable<ApplicantStatusModel> applicantStatusModel()
        {
            var applicantStatus = new List<ApplicantStatusModel>();
            bool? Resume = unitOfWork.UserRepository.Get(filter: m => m.FirstName != null && m.Id == UserId).Any() ? true : false;
            applicantStatus.Add(new ApplicantStatusModel() { Field = "Resume", Done = Resume });
            foreach (var i in unitOfWork.JobApplicationRepo.Get(filter: m => m.UserId == this.UserId))
            {
                applicantStatus.Add(new ApplicantStatusModel()
                {
                    Field = "Exam",
                    Done = i.Exam,
                    Status = unitOfWork.Applicant.Get(filter: m => m.UserId == this.UserId).Any() ? $"Your exam is ready <a href='{new UrlHelper(requestContext).Action("Index", "ApplicantExam")  }'> click here</a>" : string.Empty
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