using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevExpress.Web;
using NorthOps.Models;
using NorthOps.Models.Repository;
using NorthOps.Services.Helpers;

namespace NorthOps.Ops.Controllers
{
    [Authorize(Roles = "Administrator,Team Leader")]
    public class _RecordingsController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        public ActionResult Index()
        {
            return View();
        }

        [Route("recording/download/{recordingId}")]
        public ActionResult Download(int recordingId)
        {
            var model = unitOfWork.RecordingsRepo.Find(m => m.Id == recordingId);
            return File(model.Recording, "audio/mpeg3", $"{model.CallerNumber}-{model.CallDate}.wav");
        }
        [ValidateInput(false)]
        public ActionResult RecordingsGridViewPartial()
        {
            var model = unitOfWork.RecordingsRepo.Get();
            return PartialView("_RecordingsGridViewPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RecordingsGridViewPartialAddNew([ModelBinder(typeof(DevExpressEditorsBinder))]Recordings item)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    var uploadedFile = Session["callRecording"] as UploadedFile[];
                    item.Recording = uploadedFile?.FirstOrDefault()?.FileBytes;
                    unitOfWork.RecordingsRepo.Insert(item);
                    unitOfWork.Save();

                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            var model = unitOfWork.RecordingsRepo.Get();
            return PartialView("_RecordingsGridViewPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult RecordingsGridViewPartialUpdate(NorthOps.Models.Recordings item)
        {

            if (ModelState.IsValid)
            {
                try
                {
                   
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            var model = unitOfWork.RecordingsRepo.Get();
            return PartialView("_RecordingsGridViewPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult RecordingsGridViewPartialDelete([ModelBinder(typeof(DevExpressEditorsBinder))] int? Id)
        {

            if (Id >= 0)
            {
                try
                {
                    unitOfWork.RecordingsRepo.Delete(unitOfWork.RecordingsRepo.Find(m => m.Id == Id));
                    unitOfWork.Save();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            var model = unitOfWork.RecordingsRepo.Get();
            return PartialView("_RecordingsGridViewPartial", model);
        }

        public ActionResult AddEditRecordingPartials([ModelBinder(typeof(DevExpressEditorsBinder))]int? recordingId)
        {
            var model = unitOfWork.RecordingsRepo.Find(m => m.Id == recordingId);
            return PartialView("_AddEditRecordingPartials", model);
        }
        public ActionResult UploadControlUpload()
        {
            Session["callRecording"] = UploadControlExtension.GetUploadedFiles("UploadControl", RecordingsControllerUploadControlSettings.UploadValidationSettings, RecordingsControllerUploadControlSettings.FileUploadComplete);
            return null;
        }
    }
    public class RecordingsControllerUploadControlSettings
    {
        public static DevExpress.Web.UploadControlValidationSettings UploadValidationSettings = new DevExpress.Web.UploadControlValidationSettings()
        {
            AllowedFileExtensions = new string[] { ".mp3", ".wav" },
            MaxFileSize = 4000000
        };
        public static void FileUploadComplete(object sender, DevExpress.Web.FileUploadCompleteEventArgs e)
        {
            if (e.UploadedFile.IsValid)
            {
                // Save uploaded file to some location
            }
        }
    }

}