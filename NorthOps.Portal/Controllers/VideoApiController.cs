using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using NorthOps.Models.Repository;
using NorthOps.Services.Helpers;

namespace NorthOps.Portal.Controllers
{
    [RoutePrefix("api")]
    public class VideoApiController : ApiController
    {
        string documentId;
        UnitOfWork unitOfWork = new UnitOfWork();
        [Route("video/{documentId?}")]
        public HttpResponseMessage GetVideoContent(string documentId)
        {
            if (Request.Headers.Range != null)
            {
                var video = unitOfWork.DocumentsRepo.Fetch(m => m.Id == documentId).FirstOrDefault();
                var fileStream = new FileStream(video.Path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                HttpResponseMessage partialResponse = Request.CreateResponse(HttpStatusCode.PartialContent);
                partialResponse.Content = new ByteRangeStreamContent(fileStream, Request.Headers.Range, "application/mp4");
                return partialResponse;
            }
            else
            {
                var httpResponce = Request.CreateResponse();
                this.documentId = documentId;
                httpResponce.Content = new PushStreamContent((Action<Stream, HttpContent, TransportContext>)WriteVideoToStream);
                //  StreamVideo(unitOfWork.DocumentsRepo.Find(m => m.Id == documentId)?.Path, Request);

                return httpResponce;
            }

        }
        [Route("excel/{documentId}")]
        public HttpResponseMessage GetExcelContent(string documentId)
        {
            var httpResponce = Request.CreateResponse();
            this.documentId = documentId;
            //"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            var vm = unitOfWork.DocumentsRepo.Fetch(m => m.Id == documentId).FirstOrDefault();
            var ext = MimeTypeHelper.GetMimeType(Path.GetExtension(vm?.Path));
            httpResponce.Content = new PushStreamContent((Action<Stream, HttpContent, TransportContext>)WriteContentToStream, new MediaTypeHeaderValue(ext));

            httpResponce.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = Path.GetFileName(vm.Path)
            };
            return httpResponce;
        }
        [Route("document/{documentId}")]
        public HttpResponseMessage GetDocumentContent(string documentId)
        {
            var httpResponce = Request.CreateResponse();
            this.documentId = documentId;
            var vm = unitOfWork.DocumentsRepo.Fetch(m => m.Id == documentId).FirstOrDefault();
            var ext = MimeTypeHelper.GetMimeType(Path.GetExtension(vm?.Path));
            httpResponce.Content = new PushStreamContent((Action<Stream, HttpContent, TransportContext>)WriteContentToStream, new MediaTypeHeaderValue(ext));
            httpResponce.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = Path.GetFileName(vm.Path)
            };
            return httpResponce;
        }
        public async void WriteContentToStream(Stream outputStream, HttpContent content, TransportContext transportContext)
        {
            int bufferSize = 1000000;
            byte[] buffer = new byte[bufferSize];
            var video = unitOfWork.DocumentsRepo.Fetch(m => m.Id == documentId).FirstOrDefault();
            using (var fileStream = new FileStream(video.Path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var length = (int)fileStream.Length;
                var bytesRead = 1;
                try
                {
                    while (length > 0 && bytesRead > 0)
                    {
                        bytesRead = fileStream.Read(buffer, 0, Math.Min(length, buffer.Length));
                        await outputStream.WriteAsync(buffer, 0, bytesRead);
                        length -= bytesRead;
                    }
                }
                catch (Exception)
                {
                    return;
                }
                finally
                {
                    outputStream.Close();
                }
            }
        }

        public async void WriteVideoToStream(Stream outputStream, HttpContent content, TransportContext transportContext)
        {
            int bufferSize = 1000000;
            byte[] buffer = new byte[bufferSize];
            var video = unitOfWork.DocumentsRepo.Fetch(m => m.Id == documentId).FirstOrDefault();
            using (var fileStream = new FileStream(video.Path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var length = (int)fileStream.Length;
                var bytesRead = 1;
                try
                {
                    while (length > 0 && bytesRead > 0)
                    {
                        bytesRead = fileStream.Read(buffer, 0, Math.Min(length, buffer.Length));
                        await outputStream.WriteAsync(buffer, 0, bytesRead);
                        length -= bytesRead;
                    }
                }
                catch (Exception)
                {
                    return;
                }
                finally
                {
                    outputStream.Close();
                }
            }
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
