using System;
using System.Collections.Generic;
using System.Web.Http;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Microsoft.Practices.Unity;
using Owin;
using TaskSharper.Domain.Notification;
using TaskSharper.Service.Config;
using TaskSharper.Service.Hubs;

namespace TaskSharper.Service
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var http = new HttpConfiguration();

            // TODO:: Restrict access
            app.UseCors(CorsOptions.AllowAll);

            var not = UnityConfig.GetContainer().Resolve<INotification>();
            GlobalHost.DependencyResolver.Register(typeof(NotificationHub), () => new NotificationHub(not));

            //app.UseJwtSignalRAuthentication(authQueryKey: "authtoken");

            //app.UseJwtBearerAuthentication(
            //    new JwtBearerAuthenticationOptions
            //    {
            //        AuthenticationMode = AuthenticationMode.Active,
            //        AllowedAudiences = new[] { AppConfig.ClientId },
            //        IssuerSecurityTokenProviders = new IIssuerSecurityTokenProvider[]
            //        {
            //            new SymmetricKeyIssuerSecurityTokenProvider(AppConfig.AuthenticationIssuer,
            //                TextEncodings.Base64Url.Decode(AppConfig.ClientSecret))
            //        }
            //    });



            app.MapSignalR();

            SwaggerConfig.Configure(http);
            WebApiConfig.Configure(app, http);
        }
    }

    public class UnitySignalRDependencyResolver : DefaultDependencyResolver
    {

        public override object GetService(Type serviceType)
        {
            object obj;
            try
            {
                obj = UnityConfig.GetContainer().Resolve(serviceType);
            }
            catch (Exception)
            {
                obj = base.GetService(serviceType);
            }

            return obj;
        }

        public override IEnumerable<object> GetServices(Type serviceType)
        {
            IEnumerable<object> obj;
            try
            {
                obj = UnityConfig.GetContainer().ResolveAll(serviceType);
            }
            catch (Exception)
            {
                obj = base.GetServices(serviceType);
            }

            return obj;
        }
    }
}
