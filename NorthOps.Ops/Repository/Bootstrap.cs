using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NorthOps.Ops.Repository
{
    public class Bootstrap
    {
        public static string beginRow = "<div class='row'>";
        public static string endDiv = "</div>";
        public static string colLg12 = "<div class='col-lg-12'>";
        public static string colLg10 = "<div class='col-lg-10'>";
        public static string colLg8 = "<div class='col-lg-8'>";
        public static string colLg6 = "<div class='col-lg-6'>";
        public static string colLg4 = "<div class='col-lg-4'>";
        public static string colLg2 = "<div class='col-lg-2'>";
        public static System.Web.UI.WebControls.Unit FullWidth = System.Web.UI.WebControls.Unit.Percentage(100);


        public static string colMd12 = "<div class='col-md-12'>";
        public static string colXm12 = "<div class='col-xm-12'>";
        public static void Write(string Content)
        {
            System.Web.Mvc.ViewContext viewContext = new System.Web.Mvc.ViewContext();
            viewContext.Writer.Write(Content);
        }
        private static Random random = new Random();
        public static string randomString()
        {
            const string chars = "abcdefghijklqmopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, 3)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}