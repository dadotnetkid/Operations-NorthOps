

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using NorthOps.Models.Repository;

namespace Northops.WebApi.Controllers
{
    [Authorize]
    public class AudioController : ApiController
    {


        
        public HttpResponseMessage Get()
        {
            //HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            //var stream = new FileStream(HttpContext.Current.Server.MapPath("~/files/audio.mp3"), FileMode.Open, FileAccess.Read);
            //result.Content = new StreamContent(stream);
            //result.Content.Headers.Add("Content-Type", "application/octet-stream");
            var response = Request.CreateResponse();
            response.Content = new PushStreamContent((Action<Stream, HttpContent, TransportContext>) WriteToStream, new MediaTypeHeaderValue("audio/mp3"));

            return response;
        }


        private UnitOfWork unitOfWork = new UnitOfWork();
        public async void WriteToStream(Stream outputStream, HttpContent content, TransportContext context)
        {
            try
            {
                var buffer = new byte[65536];

                using (var video = new FileStream(HttpContext.Current.Server.MapPath("~/files/audio.mp3"), FileMode.Open, FileAccess.Read))
                {
                    var length = (int)video.Length;
                    var bytesRead = 1;

                    while (length > 0 && bytesRead > 0)
                    {
                        bytesRead = video.Read(buffer, 0, Math.Min(length, buffer.Length));
                        await outputStream.WriteAsync(buffer, 0, bytesRead);
                        length -= bytesRead;
                    }
                }
            }
            catch (HttpException ex)
            {
                return;
            }
            finally
            {
                outputStream.Close();
            }
        }




    }
}
