using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthOps.Models.ViewModels
{
    public class IdentificationExamViewModel
    {
        public Guid QuestionId { get; set; }
        public string SessionId { get; set; }
        public string Choice { get; set; }
        public string CorrectAnswer { get; set; }
        public int Item { get; set; }
        public Questions Questions { get; set; }
        public int Score { get; set; }
    }
}
