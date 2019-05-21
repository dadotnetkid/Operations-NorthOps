using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NorthOps.Models.Repository;

namespace NorthOps.Models
{
    public partial class Exams
    {
        public IEnumerable<Questions> MBTIExams
        {
            get
            {
                return this.Questions.Where(m => m.Choices.Count() > 0).OrderBy(m => Convert.ToInt32(m.Title.Replace("MBTI", "")));
            }
        }
        public IEnumerable<Questions> RandomQuestion
        {
            get { return this.Questions.Skip(0).Take(50).Where(m => m.Choices.Any()).OrderBy(m => Guid.NewGuid()); }
        }
        public IEnumerable<Questions> QuestionsList
        {
            get
            {
                return this.IsRandom == true ? this.RandomQuestion : this.MBTIExams;
            }
        }

        public string Type
        {
            get
            {
                switch ((ExamTypes)this.ExamType)
                {
                    case ExamTypes.Multiple:
                        return "Multiple Choice";
                    case ExamTypes.TrueorFalse:
                        return "True or False";
                    case ExamTypes.MBTI:
                        return "Behavioural Test";
                    case ExamTypes.Listening:
                        return "Listening Skill";
                    case ExamTypes.TypingSkills:
                        return "Typing Skills";
                    default:
                        return "";
                }
            }
        }
        public string GetExamType(int? type)
        {
            switch ((ExamTypes)type)
            {
                case ExamTypes.Multiple:
                    return "Multiple Choice";
                case ExamTypes.TrueorFalse:
                    return "True or False";
                case ExamTypes.MBTI:
                    return "Behavioural Test";
                case ExamTypes.Listening:
                    return "Listening Skill";
                case ExamTypes.TypingSkills:
                    return "Typing Skills";
                default:
                    return "";
            }
        }
        public Dictionary<string, string> Answer { get { return _Answer; } set { value = _Answer; } }
        private Dictionary<string, string> _Answer = new Dictionary<string, string>();
       // public IEnumerable<Categories> Categories => new UnitOfWork().CategoryRepo.Get();
    }
    public enum ExamTypes
    {
        Multiple,
        TrueorFalse,
        MBTI,
        Listening,
        TypingSkills,
        Identification

    }
    public partial class ExamType
    {
        public static string value(int val)
        {
            switch (val)
            {
                case 0:
                    return "Multiple Choice";
                case 1:
                    return "True or False";
                case 2:
                    return "Personality Exam";
                case 3:
                    return "Listening Exam";
                case 4:
                    return "Typing Speed";
                default:
                    return "";
                    break;
            }
        }
    }
}
