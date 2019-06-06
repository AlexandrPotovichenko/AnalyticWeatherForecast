
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WeatherForecast.App_Start;
using WeatherForecast.Models.Context;
using WeatherForecast.Models.WeatherSources;
using WeatherForecast.Models.Log;

namespace WeatherForecast
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static string message;
        protected void Application_Start()
        {
            Database.SetInitializer(new WeatherContextInitializer());
            //Database.SetInitializer(new MigrateDatabaseToLatestVersion<WeatherContext, Configuration>(true, new Configuration()));
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);


            Logger.InitLogger();

            ////////////// Application.Lock();
            ////////////// Application.Add("ApplicationState", null);
            ////////////// //Application["Error"] =
            //////////////     //((int)Application["PageRequestCount"]) + 1;
            ////////////// Application.UnLock();
            Scheduler.Start();
        }
    }
}
