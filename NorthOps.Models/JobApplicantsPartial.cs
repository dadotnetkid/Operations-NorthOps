using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthOps.Models
{
   public partial class JobApplications
    {
        public string MBTIResult
        {
            get
            {
                var mbti = this.Users.PersonalityResults.FirstOrDefault();
                return mbti == null ? "" : ((mbti.E ?? 0) > (mbti.I ?? 0) ? "E" : "I") + ((mbti.S ?? 0) > (mbti.N ?? 0) ? "S" : "N") + "" + ((mbti.T ?? 0) > (mbti.F ?? 0) ? "T" : "F") + ((mbti.J ?? 0) > (mbti.P ?? 0) ? "J" : "P");
            }
        }
        public double? ApplicantExamScore
        {
            get
            {

                var result = this.Users.Applicants.Where(m => m.UserId == this.UserId && m.Result != null).Sum(m => ((m.Result ?? 0.0) / (m.Exams.Items ?? 0.0)) * (m.Exams.Percentage ?? 0.0));
                return result;
            }
        }
    
    }
}
