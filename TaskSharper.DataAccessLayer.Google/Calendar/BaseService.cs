using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using TaskSharper.DataAccessLayer.Authentication;

namespace TaskSharper.DataAccessLayer.Calendar
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
