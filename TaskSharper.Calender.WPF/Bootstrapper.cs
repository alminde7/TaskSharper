using System;
using System.Windows;
using Google.Apis.Auth.OAuth2;
using Prism.Unity;
using Microsoft.Practices.Unity;
using Prism.Logging;
using Prism.Regions;
using Serilog;
using TaskSharper.Calender.WPF.Views;
using TaskSharper.DataAccessLayer.Google.Calendar.Service;
using TaskSharper.Domain.Calendar;
using TaskSharper.Shared.Logging;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using TaskSharper.DataAccessLayer.Google;
using TaskSharper.DataAccessLayer.Google.Authentication;

namespace TaskSharper.Calender.WPF
{
    public class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow.Show();

            // Set default Calendar on start.
            var regionManager = Container.Resolve<IRegionManager>();
            regionManager.RequestNavigate("CalendarRegion", "CalendarWeekView");

        }
        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            Container.RegisterTypeForNavigation<CalendarTodayView>("CalendarTodayView");
            Container.RegisterTypeForNavigation<CalendarWeekView>("CalendarWeekView");
            Container.RegisterTypeForNavigation<CalendarMonthView>("CalendarMonthView");

            var logger = LogConfiguration.Configure();

            var googleService = new CalendarService(new BaseClientService.Initializer()
            {
                ApplicationName = Constants.TaskSharper,
                HttpClientInitializer = new GoogleAuthentication(logger).Authenticate()
            });

            Container.RegisterInstance(typeof(CalendarService), googleService);
            Container.RegisterInstance(typeof(ILogger), logger);
            Container.RegisterType<ICalendarService, GoogleCalendarService>();
        }

        protected override ILoggerFacade CreateLogger()
        {
            return new SerilogLogger();
        }
    }

    public class SerilogLogger : ILoggerFacade
    {
        public void Log(string message, Category category, Priority priority)
        {
            switch (category)
            {
                case Category.Debug:
                    Serilog.Log.Logger.Debug(message);
                    break;
                case Category.Exception:
                    Serilog.Log.Logger.Error(message);
                    break;
                case Category.Info:
                    Serilog.Log.Logger.Information(message);
                    break;
                case Category.Warn:
                    Serilog.Log.Logger.Warning(message);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(category), category, null);
            }
        }
    }
}
