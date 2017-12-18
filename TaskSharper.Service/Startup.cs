using System;
using System.Collections.Generic;
using System.Web.Http;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Microsoft.Practices.Unity;
using Owin;
using Serilog;
using TaskSharper.Service.Config;
using TaskSharper.Service.Hubs;
using TaskSharper.Service.Middleware;

namespace TaskSharper.Service
{
    public class Startup
    {
        /// <summary>
        /// Configures selfhosting application
        /// </summary>
        /// <param name="app"></param>
        public void Configuration(IAppBuilder app)
        {
            var http = new HttpConfiguration();

            // TODO:: Restrict access
            app.UseCors(CorsOptions.AllowAll);
            
            var logger = UnityConfig.GetContainer().Resolve<ILogger>();

            // https://docs.microsoft.com/en-us/aspnet/signalr/overview/advanced/dependency-injection
            GlobalHost.DependencyResolver.Register(typeof(NotificationHub), () => new NotificationHub(logger));

            // This is to enable logging from LogAttribute. The logger cannot be dependencyinjected
            Log.Logger = logger;

            app.MapSignalR();

            app.Use(typeof(CorrelationIdMiddleware));

            SwaggerConfig.Configure(http);
            WebApiConfig.Configure(app, http);
        }
    }
}
