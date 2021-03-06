﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using NorthOps.Models.Config;

namespace Northops.WebApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.RegisterWithOdata);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
        void Application_BeginRequest(object sender, EventArgs e)
        {
            if (ReferenceEquals(null, HttpContext.Current.Request.Headers["Authorization"]))
            {
                var token = HttpContext.Current.Request.Params["access_token"];
                if (!String.IsNullOrEmpty(token))
                {
                    HttpContext.Current.Request.Headers.Add("Authorization", "Bearer " + token);
                }
            }
        }
    }
}
