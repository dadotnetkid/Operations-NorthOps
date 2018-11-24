//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NorthOps.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class JobApplications
    {
        public System.Guid JobApplicationId { get; set; }
        public string UserId { get; set; }
        public Nullable<bool> Resume { get; set; }
        public Nullable<bool> PhoneInterview { get; set; }
        public Nullable<bool> PersonalInterview { get; set; }
        public Nullable<bool> Training { get; set; }
        public Nullable<bool> OnBoarding { get; set; }
        public Nullable<bool> Contract { get; set; }
        public Nullable<decimal> ExamScore { get; set; }
        public Nullable<bool> Exam { get; set; }
        public Nullable<System.DateTime> PhoneInterviewDate { get; set; }
        public Nullable<System.DateTime> PersonalInterviewDate { get; set; }
        public Nullable<System.DateTime> TrainingDate { get; set; }
        public Nullable<System.DateTime> OnBoardingDate { get; set; }
        public Nullable<System.DateTime> ContractDate { get; set; }
        public Nullable<bool> IsPersonalInterviewPassed { get; set; }
        public Nullable<bool> IsExamPassed { get; set; }
        public Nullable<bool> IsPhoneInterviewPassed { get; set; }
        public Nullable<bool> IsTrainingPassed { get; set; }
        public Nullable<bool> IsOnBoardingPassed { get; set; }
        public Nullable<System.DateTime> IsExamPassedDate { get; set; }
    
        public virtual Users Users { get; set; }
    }
}
