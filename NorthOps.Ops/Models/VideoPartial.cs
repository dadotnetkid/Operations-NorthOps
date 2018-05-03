using NorthOps.Ops.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace NorthOps.Ops.Models
{
    public class VideoPartial
    {
    }
    public partial class Video
    {

        public IEnumerable<Video> VideoList
        {
            get
            {
                var list = new DirectoryInfo(HttpContext.Current.Server.MapPath("~/filemanager/videos")).EnumerateFiles().Select(x => new Video { VideoPath = $"{VirtualPathUtility.ToAbsolute("~/filemanager/videos")}/{Path.GetFileName(x.FullName)}", VideoName = Path.GetFileName(x.FullName) });
                return list;
            }
        }
        public IEnumerable<Exam> Exams
        {
            get { return new UnitOfWork().ExamRepo.Get(); }
        }
    }
}