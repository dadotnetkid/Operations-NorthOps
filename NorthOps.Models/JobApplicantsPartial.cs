using NorthOps.Models.Repository;
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
        public decimal? ApplicantExamScore
        {
            get
            {
                var result = (Listening * 0.5m) + (Grammar * 1.0M);

                // + ((Grammar * 0.0M / GrammarDetailResult?.Exams?.Items * 0.0M) * (GrammarDetailResult?.Exams?.Percentage/100.0M) ?? 0.0M);
                //foreach (var i in this.Users.Applicants.Where(m => m.Exams.ExamName != "Typing Speed"))
                //{
                //    var retval = (i.Result * 1.0M / i.Exams.Items * 1.0M) * (i.Exams.Percentage * 1.0M);
                //    if (retval != null) result += retval.Value;
                //}

                // result = this.Users.Applicants.Where(m => m.UserId == this.UserId && m.Result != null).Sum(m => ((m.Result ?? 0.0M) / (m.Exams.Items ?? 0.0M)) * (m.Exams.Percentage ?? 0.0M));
                return result;
            }
        }

        public decimal? Listening
        {
            get
            {
                var result = this.Users.Applicants
                    .FirstOrDefault(m => m.Exams.Categories.CategoryName == ("Listening Skills"))?.Result;
                UnitOfWork unitOfWork = new UnitOfWork();
                var equivalent = unitOfWork.EquivalentsRepo.Find(m => m.Score == result);

                return equivalent?.Equivalent;
            }
        }
        public decimal? Typing
        {
            get
            {
                return this.Users.Applicants
                    .FirstOrDefault(m => m.Exams.Categories.CategoryName == ("Typing Skills"))?.Result;
            }
        }
        public decimal? Grammar
        {
            get
            {
                return this.Users.Applicants
                    .FirstOrDefault(m => m.Exams.Categories.CategoryName == ("Grammar and Vocabulary"))?.Result;
            }
        }
        public Applicants GrammarDetailResult
        {
            get
            {
                var res = this.Users.Applicants
                    .FirstOrDefault(m => m.Exams.Categories.CategoryName == ("Grammar and Vocabulary"));
                return res;
            }
        }
        public bool? IsExamReady
        {
            get
            {
                return this.Users?.Applicants.Any();
            }
        }
    }
}
