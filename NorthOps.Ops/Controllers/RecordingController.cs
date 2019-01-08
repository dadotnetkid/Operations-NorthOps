using System;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using DevExpress.Web;
using DevExpress.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using NorthOps.AspIdentity;
using NorthOps.Models;
using NorthOps.Models.Repository;
using NorthOps.Models.ViewModels;
using NorthOps.Services.Helpers;

namespace NorthOps.Ops.Controllers
{
    [Authorize(Roles = "Employee")]
    public class RecordingController : Controller
    {

        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            private set { _userManager = value; }

        }

        public string userId
        {
            get { return User.Identity.GetUserId(); }
        }

        private UnitOfWork unitOfWork = new UnitOfWork();

        private ApplicationUserManager _userManager;

        // GET: Recording
        public ActionResult Index()
        {
            return View();
        }

        [ValidateInput(false)]
        public ActionResult RecordingGridViewPartial()
        {
            var model = unitOfWork.RecordingsRepo.Fetch();
            if (!User.IsInRoles("Team Leader", "Administrator"))
            {
                model = model.Where(m => m.UserId == userId);
            }
            return PartialView("_RecordingGridViewPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RecordingGridViewPartialAddNew([ModelBinder(typeof(DevExpressEditorsBinder))]Recordings item)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    if (!User.IsInRoles("Administrator"))
                    {
                        item.UserId = string.IsNullOrEmpty(item.UserId) ? User.Identity.GetUserId() : item.UserId;
                        item.isAgentAcknowledge = true;
                    }
                    else
                    {
                        item.isAdministratorAcknowledge = true;
                    }

                    var UserId = User.Identity.GetUserId();
                    item.AcknowledgeToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(
                        new AcknowledgeTokenViewModel()
                        {
                            Password = UserManager.PasswordHasher.HashPassword(item.AcknowledgeTokenViewModel.Password),
                            AcknowledgeDate = DateTime.Now,
                            AcknowledgeBy = unitOfWork.UserRepository.Get(m => m.Id == userId).Select(x => new { FirstName = x.FirstName, MiddleName = x.MiddleName, LastName = x.LastName }).FirstOrDefault(),
                            ModifiedBy = unitOfWork.UserRepository.Get(m => m.Id == userId).Select(x => new { FirstName = x.FirstName, MiddleName = x.MiddleName, LastName = x.LastName }).FirstOrDefault()
                        })));

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
            var model = unitOfWork.RecordingsRepo.Fetch();
            if (!User.IsInRoles("Team Leader", "Administrator"))
            {
                model = model.Where(m => m.UserId == userId);
            }
            return PartialView("_RecordingGridViewPartial", model.ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult RecordingGridViewPartialUpdate(NorthOps.Models.Recordings item)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    var uploadedFile = Session["callRecording"] as UploadedFile[];
                    item.Recording = uploadedFile?.FirstOrDefault()?.FileBytes;
                    var recording = unitOfWork.RecordingsRepo.Find(m => m.Id == item.Id);
                    if (User.IsInRoles("Administrator", "Team Leader"))
                    {
                        recording.CampaignId = item.CampaignId == recording.CampaignId
                             ? recording.CampaignId
                             : item.CampaignId;
                        recording.UserId = item.UserId == recording.UserId
                            ? recording.UserId
                            : item.UserId;
                        recording.Recording = item.Recording == recording.Recording
                            ? recording.Recording
                            : item.Recording ?? recording.Recording;
                        recording.CallDate = item.CallDate == recording.CallDate
                            ? recording.CallDate
                            : item.CallDate;
                        recording.Errors = item.Errors == recording.Errors
                                                 ? recording.Errors
                                                 : item.Errors;
                        recording.CallerNumber = item.CallerNumber == recording.CallerNumber
                                                                      ? recording.CallerNumber
                                                                      : item.CallerNumber;
                        recording.ErrorTypeId = item.ErrorTypeId == recording.ErrorTypeId
                            ? recording.ErrorTypeId
                            : item.ErrorTypeId;
                        recording.ErrorDateTime = item.ErrorDateTime == recording.ErrorDateTime
                            ? recording.ErrorDateTime
                            : item.ErrorDateTime;


                        recording.AcknowledgmentDate = item.AcknowledgmentDate == recording.AcknowledgmentDate
                            ? recording.AcknowledgmentDate
                            : item.AcknowledgmentDate;
                        recording.CoachingDate = item.CoachingDate == recording.CoachingDate
                            ? recording.CoachingDate
                            : item.CoachingDate;
                    }
                    recording.RootCauseAnalysis = item.RootCauseAnalysis == recording.RootCauseAnalysis
                        ? recording.RootCauseAnalysis
                        : item.RootCauseAnalysis;
                    recording.Commitment = item.Commitment == recording.Commitment
                        ? recording.Commitment
                        : item.Commitment;
                    recording.AcknowledgmentDate = DateTime.Now;






                    if (!User.IsInRoles("Administrator"))
                    {
                        item.UserId = string.IsNullOrEmpty(item.UserId) ? User.Identity.GetUserId() : item.UserId;
                        item.isAgentAcknowledge = true;
                    }
                    else
                    {
                        item.isAdministratorAcknowledge = true;
                    }


                    item.AcknowledgeToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(
                        new AcknowledgeTokenViewModel()
                        {
                            Password = UserManager.PasswordHasher.HashPassword(item.AcknowledgeTokenViewModel.Password),
                            AcknowledgeDate = DateTime.Now,
                            AcknowledgeBy = unitOfWork.UserRepository.Get(m => m.Id == userId).Select(x => new { FirstName = x.FirstName, MiddleName = x.MiddleName, LastName = x.LastName }).FirstOrDefault(),
                            ModifiedBy = unitOfWork.UserRepository.Get(m => m.Id == userId).Select(x => new { FirstName = x.FirstName, MiddleName = x.MiddleName, LastName = x.LastName }).FirstOrDefault(),
                            DateModified=DateTime.Now 
                        })));




                    unitOfWork.Save();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            var model = unitOfWork.RecordingsRepo.Fetch();
            if (!User.IsInRoles("Team Leader", "Administrator"))
            {
                model = model.Where(m => m.UserId == userId);
            }
            return PartialView("_RecordingGridViewPartial", model.ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult RecordingGridViewPartialDelete([ModelBinder(typeof(DevExpressEditorsBinder))] int? Id)
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
            return PartialView("_RecordingGridViewPartial", model);
        }

        public ActionResult AddEditRecordingsPartial(int? recordingId)
        {
            var model = unitOfWork.RecordingsRepo.Find(m => m.Id == recordingId) ?? new Recordings() { Id = new Random().Next(0, 1000) };
            return PartialView("_AddEditRecordingsPartial", model);
        }

        public ActionResult DetailRecordingsPartial(int? recordingId)
        {
            return PartialView("_DetailRecordingsPartial", unitOfWork.RecordingsRepo.Find(m => m.Id == recordingId));
        }

        [Route("recording/download/{recordingId}")]
        public ActionResult Download(int recordingId)
        {
            var model = unitOfWork.RecordingsRepo.Find(m => m.Id == recordingId);
            return File(model.Recording, "audio/mpeg3", $"{model.CallerNumber}-{model.CallDate}.wav");
        }

        public ActionResult ConfirmPasswordPopupControlPartial([ModelBinder(typeof(DevExpressEditorsBinder))] Recordings model)
        {
            return PartialView("_ConfirmPasswordPopupControlPartial", model);
        }

        public ActionResult UploadControlUpload()
        {
            var file = UploadControlExtension.GetUploadedFiles("UploadControl", RecordingControllerUploadControlSettings.UploadValidationSettings, RecordingControllerUploadControlSettings.FileUploadComplete);
            ViewBag.FileName = file.FirstOrDefault()?.FileName;
            Session["callRecording"] = file;
            return null;
        }

        public JsonResult CheckPassword(string password)
        {
            var res = UserManager.CheckPassword(UserManager.FindById(userId), password);
            return Json(res, JsonRequestBehavior.AllowGet);
        }


    }
    public class RecordingControllerUploadControlSettings
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
                e.CallbackData = e.UploadedFile.FileName;
            }
        }
    }

}