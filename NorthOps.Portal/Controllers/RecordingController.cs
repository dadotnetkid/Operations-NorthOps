using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using NorthOps.Models.Repository;

namespace NorthOps.Portal.Controllers
{
    [Authorize(Roles ="Employee")]
    public class RecordingController : Controller
    {
        public string userId
        {
            get { return User.Identity.GetUserId(); }
        }

        private UnitOfWork unitOfWork = new UnitOfWork();
        // GET: Recording
        public ActionResult Index()
        {
            return View();
        }

        [ValidateInput(false)]
        public ActionResult RecordingGridViewPartial()
        {
            var model = unitOfWork.RecordingsRepo.Get(m => m.UserId == userId);
            return PartialView("_RecordingGridViewPartial", model);
        }

        [Route("recording/download/{recordingId}")]
        public ActionResult Download(int recordingId)
        {
            var model = unitOfWork.RecordingsRepo.Find(m => m.Id == recordingId);
            return File(model.Recording, "audio/mpeg3", $"{model.Number}-{model.CallDate}.wav");
        }


    }
}