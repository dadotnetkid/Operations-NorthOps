using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NorthOps.Ops.Models
{
    public partial class JobApplication
    {
        public string MBTIResult
        {
            get
            {
                var mbti = this.User.PersonalityResults.FirstOrDefault();
                return mbti == null ? "" : ((mbti.E ?? 0) > (mbti.I ?? 0) ? "E" : "I") + ((mbti.S ?? 0) > (mbti.N ?? 0) ? "S" : "N") + "" + ((mbti.T ?? 0) > (mbti.F ?? 0) ? "T" : "F") + ((mbti.J ??0) > (mbti.P ??0) ? "J" : "P");
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