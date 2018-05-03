using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NorthOps.Portal.Models
{
    public partial class JobApplication
    {
        public string MBTIResult
        {
            get
            {
                var mbti = this.User.PersonalityResults.FirstOrDefault();
                return mbti == null ? "" : (mbti.E > mbti.I ? "E" : "I") + (mbti.S > mbti.N ? "S" : "N") + "" + (mbti.T > mbti.F ? "T" : "F") + (mbti.J > mbti.P ? "J" : "P");
            }
        }
        public double? ApplicantExamScore
        {
            get
            {

                var result = this.User.Applicants.Where(m => m.UserId == this.UserId && m.Result != null).Sum(m => ((m.Result ?? 0.0) / (m.Exam.Items ?? 0.0)) * (m.Exam.Percentage ?? 0.0));
                return result;
            }
        }
    }
}