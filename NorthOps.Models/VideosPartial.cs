using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using NAudio.Wave;
using NorthOps.Models.Repository;

namespace NorthOps.Models
{
    public partial class Videos
    {

        public IEnumerable<Videos> VideoList
        {
            get
            {
                var list = new DirectoryInfo(HttpContext.Current.Server.MapPath("~/filemanager/videos")).EnumerateFiles().Select(x => new Videos { VideoPath = $"{VirtualPathUtility.ToAbsolute("~/filemanager/videos")}/{Path.GetFileName(x.FullName)}", VideoName = Path.GetFileName(x.FullName) });
                return list;
            }
        }
        public IEnumerable<Exams> ExamsList
        {
            get { return new UnitOfWork().ExamRepo.Get(); }
        }

        public string Size
        {
            get
            {
                var size = this.Video?.Length / 1024;
                return (size ?? 0) + " Kb";
            }
        }

        public int AudioLength
        {
            get
            {
                Mp3FileReader reader = new Mp3FileReader(new MemoryStream(this.Video));

                return reader?.TotalTime.Seconds ?? 0;

            }
        }
    }
}
