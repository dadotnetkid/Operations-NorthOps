using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevExpress.Web;
using NorthOps.Models.Repository;

namespace NorthOps.Ops.Controllers
{
    public class RecordingsController : Controller
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
            return File(model.Recording, "audio/mpeg3", $"{model.Number}-{model.CallDate}.wav");
        }
        [ValidateInput(false)]
        public ActionResult RecordingsGridViewPartial()
        {
            var model = unitOfWork.RecordingsRepo.Get();
            return PartialView("_RecordingsGridViewPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RecordingsGridViewPartialAddNew(NorthOps.Models.Recordings item)
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
            var model = new object[0];
            if (ModelState.IsValid)
            {
                try
                {
                    // Insert here a code to update the item in your model
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("_RecordingsGridViewPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult RecordingsGridViewPartialDelete(System.Int32 Id)
        {
            var model = new object[0];
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
            return PartialView("_RecordingsGridViewPartial", model);
        }

        public ActionResult UploadControlUpload()
        {
            Session["callRecording"] = UploadControlExtension.GetUploadedFiles("UploadControl", CallRecordingControllerUploadControlSettings.UploadValidationSettings, CallRecordingControllerUploadControlSettings.FileUploadComplete);
            return null;
        }
    }
    public class CallRecordingControllerUploadControlSettings
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