using NorthOps.Ops.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NorthOps.Ops.Models
{
    public class QuestionPartial
    {
    }
    public partial class Question
    {
        public IEnumerable<Video> Videos { get { return new UnitOfWork().VideoRepo.Get(); } }
    }
}