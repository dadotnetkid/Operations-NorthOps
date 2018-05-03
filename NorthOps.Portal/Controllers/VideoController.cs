using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using NorthOps.Models.Repository;

namespace NorthOps.Portal.Controllers
{
    [RoutePrefix("api")]
    public class VideoController : ApiController
    {
        Guid? VideoId;
        UnitOfWork unitOfWork = new UnitOfWork();
        [Route("video/{VideoId?}")]
        public HttpResponseMessage GetVideoContent(Guid? VideoId)
        {
            var httpResponce = Request.CreateResponse();
            this.VideoId = VideoId;
            httpResponce.Content = new PushStreamContent((Action<Stream, HttpContent, TransportContext>)WriteContentToStream);
            return httpResponce;
        }
        public async void WriteContentToStream(Stream outputStream, HttpContent content, TransportContext transportContext)
        {
            //path of file which we have to read//  

            //here set the size of buffer, you can set any size  
            int bufferSize = 1000;
            byte[] buffer = new byte[bufferSize];
            //here we re using FileStream to read file from server//  

            var video = await unitOfWork.VideoRepo.GetByIDAsync(VideoId);
            using (var fileStream = new MemoryStream(video.Video))
            {
                int totalSize = (int)fileStream.Length;
                try
                {
                    /*here we are saying read bytes from file as long as total size of file 

                    is greater then 0*/
                    while (totalSize > 0)
                    {
                        int count = totalSize > bufferSize ? bufferSize : totalSize;
                        //here we are reading the buffer from orginal file  
                        int sizeOfReadedBuffer = fileStream.Read(buffer, 0, count);
                        //here we are writing the readed buffer to output//  

                        await outputStream.WriteAsync(buffer, 0, sizeOfReadedBuffer);


                        //and finally after writing to output stream decrementing it to total size of file.  
                        totalSize -= sizeOfReadedBuffer;
                    }
                }
                catch (Exception)
                {
                }
            }
        }
    }

}
