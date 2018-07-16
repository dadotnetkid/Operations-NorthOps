using DevExpress.Web.Mvc;
using NorthOps.Ops.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using NorthOps.Models;
using NorthOps.Models.Repository;
using NorthOps.Services;

namespace NorthOps.Ops.Controllers
{
    public class ExamController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        public ActionResult Index()
        {
            return View();
        }



        public ActionResult ExportUploadUpload(Guid ExamId, string Name)
        {
            ImportUploadService.ExamId = ExamId;
            var res = UploadControlExtension.GetUploadedFiles(Name, ImportUploadService.UploadValidationSettings, ImportUploadService.FileUploadComplete);
            return null;
        }



        public ActionResult ExamDescriptionEditorrPartial()
        {
            return PartialView("_ExamDescriptionEditorrPartial");
        }
        public ActionResult ExamDescriptionEditorrPartialImageSelectorUpload()
        {
            HtmlEditorExtension.SaveUploadedImage("Description", ExamControllerExamDescriptionEditorSettings.ImageSelectorSettings);
            return null;
        }
        public ActionResult ExamDescriptionEditorrPartialImageUpload()
        {
            HtmlEditorExtension.SaveUploadedFile("Description", ExamControllerExamDescriptionEditorSettings.ImageUploadValidationSettings, ExamControllerExamDescriptionEditorSettings.ImageUploadDirectory);
            return null;
        }




       






        #region Categories
        public ActionResult Categories()
        {
            return View();
        }
        public ActionResult CategoryViewPartial()
        {
            var model = new object[0];
            return PartialView("_CategoryViewPartial", unitOfWork.CategoryRepo.Get());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CategoryViewPartialAddNew(Categories item)
        {
            var model = new object[0];
            if (ModelState.IsValid)
            {
                try
                {
                    item.CategoryId = Guid.NewGuid();
                    unitOfWork.CategoryRepo.Insert(item);
                    unitOfWork.Save();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("_CategoryViewPartial", unitOfWork.CategoryRepo.Get());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult CategoryViewPartialUpdate(Categories item)
        {
            var model = new object[0];
            if (ModelState.IsValid)
            {
                try
                {
                    unitOfWork.CategoryRepo.Update(item);
                    unitOfWork.Save();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("_CategoryViewPartial", unitOfWork.CategoryRepo.Get());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult CategoryViewPartialDelete(System.Guid CategoryId)
        {
            var model = new object[0];
            if (CategoryId != null)
            {
                try
                {
                    Categories addressTownCity = unitOfWork.CategoryRepo.GetByID(CategoryId);
                    unitOfWork.CategoryRepo.Delete(CategoryId);
                    unitOfWork.Save();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            return PartialView("_CategoryViewPartial", unitOfWork.CategoryRepo.Get());
        }

        #endregion
        #region Examinations

        public ActionResult ExamAddeditPartial(System.Nullable<Guid> ExamId)
        {
            return PartialView("_examaddeditpartial", unitOfWork.ExamRepo.GetByID(ExamId) ?? new Exams());
        }

        public ActionResult Examinations()
        {
            return View();
        }
        [ValidateInput(false)]
        public ActionResult ExamGridPartial()
        {
            return PartialView("_ExamGridPartial", unitOfWork.ExamRepo.Get(includeProperties: "Categories"));
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ExamGridPartialAddNew(Exams item)
        {
            item.DateCreated = DateTime.Now;
            item.ExamId = Guid.NewGuid();
            if (ModelState.IsValid)
            {
                try
                {
                    unitOfWork.ExamRepo.Insert(item);
                    unitOfWork.Save();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("_ExamGridPartial", unitOfWork.ExamRepo.Get());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult ExamGridPartialUpdate(Exams item)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    unitOfWork.ExamRepo.Update(item);
                    unitOfWork.Save();

                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("_ExamGridPartial", unitOfWork.ExamRepo.Get());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult ExamGridPartialDelete(System.Guid ExamId)
        {
            var model = new object[0];
            if (ExamId != null)
            {
                try
                {
                    var exam = unitOfWork.ExamRepo.GetByID(ExamId);
                    unitOfWork.ExamRepo.Delete(exam);
                    unitOfWork.Save();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            return PartialView("_ExamGridPartial", unitOfWork.ExamRepo.Get());
        }
        #endregion

        #region Choices

        [ValidateInput(false)]
        public ActionResult ChoiceGridViewPartial(System.Nullable<Guid> QuestionId, System.Nullable<Guid> ExamId)
        {
            ViewBag.QuestionId = QuestionId;
            ViewBag.ExamId = ExamId;
            var model = new object[0];
            return PartialView("_ChoiceGridViewPartial", unitOfWork.ChoiceRepo.Get(filter: m => m.QuestionId == QuestionId));
        }

        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> ChoiceGridViewPartialAddNew(Choices item, System.Nullable<Guid> ExamId)
        {
            var model = new object[0];
            if (ModelState.IsValid)
            {
                try
                {
                    item.ChoiceId = Guid.NewGuid();
                    unitOfWork.ChoiceRepo.Insert(item);
                    await unitOfWork.SaveAsync();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            ViewBag.QuestionId = item.QuestionId;
            ViewBag.ExamId = ExamId;
            return PartialView("_ChoiceGridViewPartial", unitOfWork.ChoiceRepo.Get(filter: m => m.QuestionId == item.QuestionId));
        }
        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> ChoiceGridViewPartialUpdate(Choices item, System.Nullable<Guid> ExamId)
        {
            var model = new object[0];
            if (ModelState.IsValid)
            {
                try
                {
                    unitOfWork.ChoiceRepo.Update(item);
                    await unitOfWork.SaveAsync();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            ViewBag.QuestionId = item.QuestionId;
            ViewBag.ExamId = ExamId;
            return PartialView("_ChoiceGridViewPartial", unitOfWork.ChoiceRepo.Get(filter: m => m.QuestionId == item.QuestionId));
        }
        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> ChoiceGridViewPartialDelete(System.Nullable<Guid> ChoiceId, System.Nullable<Guid>  QuestionId, System.Nullable<Guid> ExamId)
        {
            var model = new object[0];
            if (ChoiceId != null)
            {
                try
                {
                    unitOfWork.ChoiceRepo.Delete(await unitOfWork.ChoiceRepo.GetByIDAsync(ChoiceId));
                    await unitOfWork.SaveAsync();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            ViewBag.QuestionId = QuestionId;
            ViewBag.ExamId = ExamId;
            return PartialView("_ChoiceGridViewPartial", unitOfWork.ChoiceRepo.Get(filter: m => m.QuestionId == QuestionId));
        }

        #endregion





#region Question
        public ActionResult QuestionAddEditPartial(System.Nullable<Guid> QuestionId, System.Nullable<Guid> ExamId)
        {
            ViewBag.ExamId = ExamId.ToString();
            return PartialView("_questionAddEditPartial", unitOfWork.QuestionRepo.Get(filter: m => m.QuestionId == QuestionId && m.ExamId == ExamId).FirstOrDefault() ?? new Questions() { ExamId = ExamId.Value });
        }
        [ValidateInput(false)]
        public ActionResult QuestionGridPartial(Guid examId)
        {
            ViewBag.ExamId = examId.ToString();
            return PartialView("_QuestionGridPartial", unitOfWork.QuestionRepo.Get().Where(x => x.ExamId == examId));
        }

        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> QuestionGridPartialAddNew(Questions item)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    item.QuestionId = Guid.NewGuid();
                    unitOfWork.QuestionRepo.Insert(item);
                    await unitOfWork.SaveAsync();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            ViewBag.ExamId = item.ExamId;
            return PartialView("_QuestionGridPartial", unitOfWork.QuestionRepo.Get(filter: m => m.ExamId == item.ExamId));
        }
        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> QuestionGridPartialUpdate(Questions item)
        {
            var model = new object[0];
            if (ModelState.IsValid)
            {
                try
                {
                    unitOfWork.QuestionRepo.Update(item);
                    await unitOfWork.SaveAsync();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            ViewBag.ExamId = item.ExamId;
            return PartialView("_QuestionGridPartial", unitOfWork.QuestionRepo.Get(filter: m => m.ExamId == item.ExamId));
        }
        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> QuestionGridPartialDelete(System.Guid QuestionId, System.Guid ExamId)
        {
            var model = new object[0];
            if (QuestionId != null)
            {
                try
                {
                    unitOfWork.QuestionRepo.Delete(await unitOfWork.QuestionRepo.GetByIDAsync(QuestionId));
                    await unitOfWork.SaveAsync();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            ViewBag.ExamId = ExamId;
            return PartialView("_QuestionGridPartial", unitOfWork.QuestionRepo.Get(filter: m => m.ExamId == ExamId));
        }
        #endregion


               




        public ActionResult ExamDescriptionEditorPartial()
        {
            return PartialView("_ExamDescriptionEditorPartial");
        }
    }


    public class ExamControllerExamDescriptionEditorSettings
    {
        public const string ImageUploadDirectory = "~/Content/UploadImages/";
        public const string ImageSelectorThumbnailDirectory = "~/Content/Thumb/";

        public static DevExpress.Web.UploadControlValidationSettings ImageUploadValidationSettings = new DevExpress.Web.UploadControlValidationSettings()
        {
            AllowedFileExtensions = new string[] { ".jpg", ".jpeg", ".jpe", ".gif", ".png" },
            MaxFileSize = 4000000
        };

        static DevExpress.Web.Mvc.MVCxHtmlEditorImageSelectorSettings imageSelectorSettings;
        public static DevExpress.Web.Mvc.MVCxHtmlEditorImageSelectorSettings ImageSelectorSettings
        {
            get
            {
                if (imageSelectorSettings == null)
                {
                    imageSelectorSettings = new DevExpress.Web.Mvc.MVCxHtmlEditorImageSelectorSettings(null);
                    imageSelectorSettings.Enabled = true;
                    imageSelectorSettings.UploadCallbackRouteValues = new { Controller = "Exam", Action = "ExamDescriptionEditorrPartialImageSelectorUpload" };
                    imageSelectorSettings.CommonSettings.RootFolder = ImageUploadDirectory;
                    imageSelectorSettings.CommonSettings.ThumbnailFolder = ImageSelectorThumbnailDirectory;
                    imageSelectorSettings.CommonSettings.AllowedFileExtensions = new string[] { ".jpg", ".jpeg", ".jpe", ".gif" };
                    imageSelectorSettings.UploadSettings.Enabled = true;
                }
                return imageSelectorSettings;
            }
        }
    }





}