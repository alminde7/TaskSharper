using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Owin;
using Unity.WebApi;

namespace TaskSharper.Service.Config
{
    public class WebApiConfig
    {
        public static void Configure(IAppBuilder app, HttpConfiguration config)
        {
            //This tell WebApi where to resolve dependencies for Controllers
            config.DependencyResolver = new UnityDependencyResolver(UnityConfig.GetContainer());

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            app.UseWebApi(config);
        }
    }
}
