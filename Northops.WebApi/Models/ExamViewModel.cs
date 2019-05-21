using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Northops.WebApi.Models
{
    public class ExamViewModel
    {
        public Guid QuestionId { get; set; }
        public Guid AnswerId { get; set; }

        public Guid ExamId { get; set; }

    }

}