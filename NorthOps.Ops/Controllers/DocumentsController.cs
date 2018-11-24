using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NorthOps.Models.Repository;

namespace NorthOps.Ops.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class DocumentsController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        // GET: Documents
        public ActionResult Index()
        {
            return View();
        }

        [ValidateInput(false)]
        public ActionResult DocumentsGridViewPartial()
        {
            var model = unitOfWork.DocumentsRepo.Get();
            return PartialView("_DocumentsGridViewPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DocumentsGridViewPartialAddNew(NorthOps.Models.Documents item)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    item.DateCreated = DateTime.Now;
                    item.Id = Guid.NewGuid().ToString();
                    item.Path = System.IO.Path.Combine(DocumentsControllerFileManagerSettings.RootFolder, @item.Path);
                    unitOfWork.DocumentsRepo.Insert(item);
                    unitOfWork.Save();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";

            var model = unitOfWork.DocumentsRepo.Get();
            return PartialView("_DocumentsGridViewPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult DocumentsGridViewPartialUpdate(NorthOps.Models.Documents item)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    unitOfWork.DocumentsRepo.Update(item);
                    unitOfWork.Save();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            var model = unitOfWork.DocumentsRepo.Get();
            return PartialView("_DocumentsGridViewPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult DocumentsGridViewPartialDelete([ModelBinder(typeof(DevExpressEditorsBinder))]string Id)
        {

            if (Id != null)
            {
                try
                {
                    unitOfWork.DocumentsRepo.Delete(unitOfWork.DocumentsRepo.Find(m => m.Id == Id));
                    unitOfWork.Save();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            var model = unitOfWork.DocumentsRepo.Get();
            return PartialView("_DocumentsGridViewPartial", model);
        }

        public ActionResult cboCampaign([ModelBinder(typeof(DevExpressEditorsBinder))]string campaignId)
        {
            ViewBag.CampaignId = campaignId;
            return PartialView("_cboCampaign", unitOfWork.CampaignsRepo.Get());
        }

        public ActionResult DocumentUploadControlUpload()
        {
            UploadControlExtension.GetUploadedFiles("DocumentUploadControl", DocumentsControllerDocumentUploadControlSettings.UploadValidationSettings, DocumentsControllerDocumentUploadControlSettings.FileUploadComplete);
            return PartialView("_DocumentUploadControlUpload");
        }

        public ActionResult AddEditDocumentGridViewPartial([ModelBinder(typeof(DevExpressEditorsBinder))]string documentId)
        {
            var model = unitOfWork.DocumentsRepo.Find(m => m.Id == documentId);
            return PartialView("_AddEditDocumentGridViewPartial", model);
        }

        [ValidateInput(false)]
        public ActionResult FileManagerPartial(bool? isUploadShow = true)
        {
            ViewBag.isUploadShow = isUploadShow;
            return PartialView("_FileManagerPartial", DocumentsControllerFileManagerSettings.Model);
        }

        public ActionResult FileManagerPopupControl(bool? isSelectShow = false)
        {
            ViewBag.isSelectShow = isSelectShow;
            return PartialView("_FileManagerPopupControl");
        }
        public FileStreamResult FileManagerPartialDownload()
        {
            return FileManagerExtension.DownloadFiles("FileManager", DocumentsControllerFileManagerSettings.Model);
        }
    }
    public class DocumentsControllerFileManagerSettings
    {
        public static string RootFolder = @"\\web\c$\HostingSpaces\inhouse\documents";

        public static string Model { get { return RootFolder; } }
    }

    public class DocumentsControllerDocumentUploadControlSettings
    {
        public static DevExpress.Web.UploadControlValidationSettings UploadValidationSettings = new DevExpress.Web.UploadControlValidationSettings()
        {
            AllowedFileExtensions = new string[] { ".jpg", ".jpeg", ".pdf", ".mp4", ".docx", ".doc", ".mp3" },
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