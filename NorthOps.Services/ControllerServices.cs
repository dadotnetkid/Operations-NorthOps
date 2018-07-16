using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace NorthOps.Services
{
    public class ControllerServices
    {
        // private RouteData routeData;

        public ControllerServices()
        {

        }
        public static bool IsControllerName(params string[] ControllerName)
        {
            var httpContext = new HttpContextWrapper(HttpContext.Current);
            var routeData = System.Web.Routing.RouteTable.Routes.GetRouteData(httpContext);
            foreach (var i in ControllerName)
            {
                if (routeData != null && routeData.Values["Controller"].ToString().ToLower() == i)
                {
                    return true;

                }

            }
            return false;
        }
        public static bool IsActionName(params string[] ActionName)
        {
            var httpContext = new HttpContextWrapper(HttpContext.Current);
            var routeData = HttpContext.Current.Request.RequestContext.RouteData.Values["action"]?.ToString().ToLower();
            foreach (var i in ActionName)
            {
                if (routeData == i)
                {
                    return true;
                }

            }
            return false;
        }
    }
}
