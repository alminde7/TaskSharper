using Google.Apis.Auth.OAuth2;
using TaskSharper.DataAccessLayer.Google.Authentication;

namespace TaskSharper.DataAccessLayer.Google.Calendar
{
    public class BaseService
    {
        protected UserCredential UserCredential;
        public void Authenticate()
        {
            UserCredential = GoogleAuthentication.Authenticate();
        }
    }
}
