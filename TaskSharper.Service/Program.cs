using Serilog;
using TaskSharper.Configuration.Config;
using TaskSharper.Shared.Logging;
using Topshelf;

namespace TaskSharper.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            ILogger logger = LogConfiguration.ConfigureAPI(LoggingConfig.Get());

            HostFactory.Run(x =>
            {
                x.UseSerilog(logger);

                x.Service<Service>(s =>
                {
                    s.ConstructUsing(service => new Service());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });

                x.RunAsLocalSystem();

                x.SetServiceName("TaskSharper.Service");
                x.SetDescription("Service for handling data, cache and notification for TaskSharper clients");
                x.SetDisplayName("TaskSharper.Service");

                x.StartAutomatically();
            });
        }
    }
}
