using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Swashbuckle.Application;

namespace TaskSharper.Service.Config
{
    public class SwaggerConfig
    {
        /// <summary>
        /// Configure swagger on Rest API
        /// </summary>
        /// <param name="config"></param>
        public static void Configure(HttpConfiguration config)
        {
            Console.WriteLine("Enabled Swagger");
            config.EnableSwagger(x =>
            {
                x.SingleApiVersion("v1", "TaskSharper API");
                x.DescribeAllEnumsAsStrings();

                //x.IncludeXmlComments($@"{AppDomain.CurrentDomain.BaseDirectory}\TaskSharper.Service.XML");
            }).EnableSwaggerUi();
        }
    }
}
