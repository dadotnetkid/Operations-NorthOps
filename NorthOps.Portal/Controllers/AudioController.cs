using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NorthOps.Models.Repository;

namespace NorthOps.Portal.Controllers
{
    public class AudioController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        public ActionResult Index(string audioId)
        {
            var audio = unitOfWork.VideoRepo.Fetch(m => m.VideoId.ToString() == audioId).FirstOrDefault();
            return new FileStreamResult(new MemoryStream(audio?.Video),"audio/mp3");
        }
    }
}