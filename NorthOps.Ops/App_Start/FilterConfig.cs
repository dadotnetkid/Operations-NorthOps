using System.Web;
using System.Web.Mvc;

namespace NorthOps.Ops {
    public class FilterConfig {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
