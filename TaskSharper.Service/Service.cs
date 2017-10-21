using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using TaskSharper.Service.Config;

namespace TaskSharper.Service
{
    class Service
    {
        private IDisposable _webApp;
        private readonly string _hostUrl = $"http://+:8000";

        public void Start()
        {
            _webApp = WebApp.Start<Startup>(_hostUrl);
        }

        public void Stop()
        {
            _webApp.Dispose();
            UnityConfig.GetContainer().Dispose();
        }
    }
}
