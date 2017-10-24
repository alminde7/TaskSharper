using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskSharper.Shared.Logging;
using Topshelf;

namespace TaskSharper.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                //x.UseSerilog(logger);

                x.Service<Service>(s =>
                {
                    s.ConstructUsing(service => new Service());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });

                x.SetServiceName("TaskSharper.Service");
                x.SetDescription("Service for handling data, cache and notification for TaskSharper clients");
                x.SetDisplayName("TaskSharper.Service");

                x.StartAutomatically();
            });
        }
    }
}
