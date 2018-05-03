using DevExpress.Web;
using DevExpress.Web.Mvc;
using NorthOps.Ops.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using NorthOps.Models;
using NorthOps.Models.Repository;

namespace NorthOps.Ops.Controllers
{
    [RoutePrefix("exam")]
    public class VideoController : Controller
    {
        UnitOfWork unitOfWork = new UnitOfWork();
        [Route("videos")]
        public ActionResult Index()
        {
            return View();
        }

        [ValidateInput(false)]
        public ActionResult VideoGridViewPartial()
        {
            var model = new object[0];
            return PartialView("_VideoGridViewPartial", unitOfWork.VideoRepo.Get(includeProperties: "Exam"));
        }

        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> VideoGridViewPartialAddNew(Videos item)
        {
            var model = new object[0];
            if (ModelState.IsValid)
            {
                try
                {
                    item.VideoId = Guid.NewGuid();
                    item.Video = Session["video"] == null ? item.Video : (byte[])Session["video"];
                    item.Extension = Session["extension"] == null ? item.Extension : (string)Session["extension"];
                    unitOfWork.VideoRepo.Insert(item);
                    await unitOfWork.SaveAsync();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("_VideoGridViewPartial", unitOfWork.VideoRepo.Get(includeProperties: "Exam"));
        }
        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> VideoGridViewPartialUpdate(Videos item)
        {
            var model = new object[0];
            if (ModelState.IsValid)
            {
                try
                {
                    item.Video = Session["video"] == null ? item.Video : (byte[])Session["video"];
                    item.Extension = Session["extension"] == null ? item.Extension : (string)Session["extension"];
                    unitOfWork.VideoRepo.Update(item);
                    await unitOfWork.SaveAsync();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("_VideoGridViewPartial", unitOfWork.VideoRepo.Get(includeProperties: "Exam"));
        }
        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> VideoGridViewPartialDelete(System.Guid VideoId)
        {
            var model = new object[0];
            if (VideoId != null)
            {
                try
                {
                    unitOfWork.VideoRepo.Delete(await unitOfWork.VideoRepo.GetByIDAsync(VideoId));
                    await unitOfWork.SaveAsync();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            return PartialView("_VideoGridViewPartial", unitOfWork.VideoRepo.Get(includeProperties: "Exam"));
        }
        public ActionResult VideoAddEditPartial(System.Nullable<System.Guid> VideoId)
        {
            var model = unitOfWork.VideoRepo.GetByID(VideoId) ?? new Videos();
            return PartialView("_videoaddeditpartial", model);
        }

        public ActionResult VideoUploadControlUpload()
        {
            UploadControlExtension.GetUploadedFiles("VideoUploadControl", VideoControllerVideoUploadControlSettings.UploadValidationSettings, VideoControllerVideoUploadControlSettings.FileUploadComplete);
            return null;
        }
    }
    public class VideoControllerVideoUploadControlSettings
    {
        public static DevExpress.Web.UploadControlValidationSettings UploadValidationSettings = new DevExpress.Web.UploadControlValidationSettings()
        {
            MaxFileSize = 40000000
        };
        public static void FileUploadComplete(object sender, DevExpress.Web.FileUploadCompleteEventArgs e)
        {
            if (e.UploadedFile.IsValid)
            {
                HttpContext.Current.Session["video"] = e.UploadedFile.FileBytes;
                HttpContext.Current.Session["extension"] = System.IO.Path.GetExtension(e.UploadedFile.FileName);
                // Save uploaded file to some location
            }
        }
    }

}