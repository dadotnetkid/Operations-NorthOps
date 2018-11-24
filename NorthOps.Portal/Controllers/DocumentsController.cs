using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using DevExpress.Pdf;
using DevExpress.Utils.OAuth.Provider;
using Microsoft.AspNet.Identity;
using NorthOps.Models.Repository;

namespace NorthOps.Portal.Controllers
{

    [Authorize(Roles = "Employee")]
    public class DocumentsController : Controller
    {
        public string UserId
        {
            get => User.Identity.GetUserId();
        }
        private UnitOfWork unitOfWork = new UnitOfWork();
        [Route("documents/trainings")]
        public ActionResult Index()
        {
            return View();
        }

        [ValidateInput(false)]
        public ActionResult DocumentGridViewGridViewPartial()
        {
            var model =
                //unitOfWork.DocumentsRepo.Fetch().Where(m => m.Campaigns.UsersInCampaignShift.Any(x => x.UserId == UserId));
                unitOfWork.DocumentsRepo.Fetch().Where(m => m.Campaigns.Users.Any(x => x.Id == UserId));

            return PartialView("_DocumentGridViewGridViewPartial", model.ToList());
        }

        public ActionResult DetailDocumentGridViewGridViewPartial([ModelBinder(typeof(DevExpressEditorsBinder))]string documentId)
        {
            var model = unitOfWork.DocumentsRepo.Fetch(m => m.Id == documentId).FirstOrDefault();

            return PartialView("_DetailDocumentGridViewGridViewPartial", model);
        }

        public ActionResult GetVideo([ModelBinder(typeof(DevExpressEditorsBinder))]string documentId)
        {

            return new VideoDataResult(documentId);
        }

    }

    public class VideoDataResult : ActionResult
    {
        private string documentId;

        public VideoDataResult(string documentId)
        {
            this.documentId = documentId;
        }
        public override void ExecuteResult(ControllerContext context)
        {




            UnitOfWork unitOfWork = new UnitOfWork();



            var model = unitOfWork.DocumentsRepo.Fetch(m => m.Id == this.documentId).FirstOrDefault();
            StreamVideo(model.Path, context.HttpContext);
        }



        internal static void StreamVideo(string fullpath, HttpContextBase context)
        {
            long size, start, end, length, fp = 0;
            using (StreamReader reader = new StreamReader(fullpath))
            {

                size = reader.BaseStream.Length;
                start = 0;
                end = size - 1;
                length = size;
                context.Response.AddHeader("Accept-Ranges", "0-" + size);
              

                if (!String.IsNullOrEmpty(context.Request.ServerVariables["HTTP_RANGE"]))
                {
                    long anotherStart = start;
                    long anotherEnd = end;
                    string[] arr_split = context.Request.ServerVariables["HTTP_RANGE"].Split(new char[] { Convert.ToChar("=") });
                    string range = arr_split[1];

                    // Make sure the client hasn't sent us a multibyte range
                    if (range.IndexOf(",") > -1)
                    {
                        // (?) Shoud this be issued here, or should the first
                        // range be used? Or should the header be ignored and
                        // we output the whole content?
                        context.Response.AddHeader("Content-Range", "bytes " + start + "-" + end + "/" + size);
                        throw new HttpException(416, "Requested Range Not Satisfiable");

                    }

                    // If the range starts with an '-' we start from the beginning
                    // If not, we forward the file pointer
                    // And make sure to get the end byte if spesified
                    if (range.StartsWith("-"))
                    {
                        // The n-number of the last bytes is requested
                        anotherStart = size - Convert.ToInt64(range.Substring(1));
                    }
                    else
                    {
                        arr_split = range.Split(new char[] { Convert.ToChar("-") });
                        anotherStart = Convert.ToInt64(arr_split[0]);
                        long temp = 0;
                        anotherEnd = (arr_split.Length > 1 && Int64.TryParse(arr_split[1].ToString(), out temp)) ? Convert.ToInt64(arr_split[1]) : size;
                    }
                    /* Check the range and make sure it's treated according to the specs.
                     * http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html
                     */
                    // End bytes can not be larger than $end.
                    anotherEnd = (anotherEnd > end) ? end : anotherEnd;
                    // Validate the requested range and return an error if it's not correct.
                    if (anotherStart > anotherEnd || anotherStart > size - 1 || anotherEnd >= size)
                    {
                        context.Response.ContentType = MimeMapping.GetMimeMapping(fullpath);
                        context.Response.AddHeader("Content-Range", "bytes " + start + "-" + end + "/" + size);
                        throw new HttpException(416, "Requested Range Not Satisfiable");
                    }
                    start = anotherStart;
                    end = anotherEnd;

                    length = end - start + 1; // Calculate new content length
                    fp = reader.BaseStream.Seek(start, SeekOrigin.Begin);
                    context.Response.StatusCode = 206;
                }
            }
            // Notify the client the byte range we'll be outputting
            context.Response.AddHeader("Content-Range", "bytes " + start + "-" + end + "/" + size);
            context.Response.AddHeader("Content-Length", length.ToString());
            // Start buffered download
            context.Response.WriteFile(fullpath, fp, length);
            context.Response.End();

        }
    }
}