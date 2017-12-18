using System.Web.Http;
using Owin;
using Unity.WebApi;

namespace TaskSharper.Service.Config
{
    public class WebApiConfig
    {
        /// <summary>
        /// Configure the Rest endpoints.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="config"></param>
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
