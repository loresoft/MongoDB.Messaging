using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using MongoDB.Messaging;
using QueueBrowser.Notifications;

namespace QueueBrowser
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        private static readonly Lazy<ChangeNotificationService> _changeService = new Lazy<ChangeNotificationService>();


        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // config message queue
            MessageQueue.Default.Configure(c => c
                .Connection("MongoMessaging")
            );

            // start broadcasting changes
            _changeService.Value.Start();
        }


        protected void Application_Stop()
        {
            _changeService.Value.Stop();
        }
    }
}
