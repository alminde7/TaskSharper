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
        protected GoogleAuthentication GoogleAuthentication;

        public BaseService(ILogger logger, GoogleAuthentication googleAuthentication)
        {
            Logger = logger;
            GoogleAuthentication = googleAuthentication;
        }

        public void Authenticate()
        {
            UserCredential = GoogleAuthentication.Authenticate();
        }
    }
}
