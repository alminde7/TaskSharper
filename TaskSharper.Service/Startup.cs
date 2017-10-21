using System.Web.Http;
using Microsoft.Owin.Cors;
using Owin;
using TaskSharper.Service.Config;

namespace TaskSharper.Service
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var http = new HttpConfiguration();

            // TODO:: Restrict access
            app.UseCors(CorsOptions.AllowAll);

            //GlobalHost.DependencyResolver.Register(typeof(IUserIdProvider), () => new MyUserProvider());

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
}
