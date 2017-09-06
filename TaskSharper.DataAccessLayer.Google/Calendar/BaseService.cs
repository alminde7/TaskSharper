using Google.Apis.Auth.OAuth2;
using Serilog;
using TaskSharper.DataAccessLayer.Google.Authentication;
using TaskSharper.Shared.Logging;

namespace TaskSharper.DataAccessLayer.Google.Calendar
{
    public class BaseService
    {
        protected UserCredential UserCredential;
        protected ILogger Logger;

        public BaseService()
        {
            InitLogging();
        }

        public void Authenticate()
        {
            UserCredential = GoogleAuthentication.Authenticate();
        }

        public void InitLogging()
        {
            Logger = LogConfiguration.Configure();
        }
    }
}
