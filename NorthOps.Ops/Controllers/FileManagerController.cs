using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NorthOps.Ops.Controllers
{
    [RoutePrefix("exam")]
    public class FileManagerController : Controller
    {
        [Route("file-manager")]
        public ActionResult Index()
        {
            return View();
        }

        [ValidateInput(false)]
        public ActionResult FileManagerPartial()
        {
            return PartialView("_FileManagerPartial", FileManagerControllerFileManagerSettings.Model);
        }

        public FileStreamResult FileManagerPartialDownload()
        {
            return FileManagerExtension.DownloadFiles("FileManager", FileManagerControllerFileManagerSettings.Model);
        }
    }
    public class FileManagerControllerFileManagerSettings
    {
        public const string RootFolder = @"~\Filemanager";

        public static string Model { get { return RootFolder; } }
    }

}