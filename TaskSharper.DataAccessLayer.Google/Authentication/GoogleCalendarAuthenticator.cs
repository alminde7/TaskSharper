using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Newtonsoft.Json;
using TaskSharper.DataAccessLayer.Google.Config;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.DataAccessLayer.Google.Authentication
{
    public class GoogleCalendarAuthenticator
    {
        public GoogleCalendarAuthenticator()
        {
            
        }

        public CalendarService Authenticate(string userAuthCode)
        {
            var pathToSecret = Path.Combine(Shared.Configuration.Config.TaskSharperCredentialStore, "client_secret.json");
            if (!File.Exists(pathToSecret)) throw new ArgumentException("Could not find any client_secret.json file in " + pathToSecret);

            var clientSecrets = LoadClientSecret(pathToSecret);
            
           
            var flow = new AuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer()
            {
                ClientSecrets = new ClientSecrets()
                {
                    ClientId = clientSecrets.installed.client_id,
                    ClientSecret = clientSecrets.installed.client_secret
                },
                Scopes = new[] {CalendarService.Scope.Calendar},
                DataStore = new FileDataStore("", true)
            });

            var token = flow.ExchangeCodeForTokenAsync("user", userAuthCode, "", CancellationToken.None).Result;

            var userCreds = new UserCredential(flow, "user", token);

            var calendarService = new CalendarService(new BaseClientService.Initializer()
            {
                ApplicationName = Constants.TaskSharper,
                HttpClientInitializer = userCreds
            });

            return calendarService;
        }

        internal ClientSecretModel LoadClientSecret(string path)
        {
            if (!File.Exists(path))
                throw new ArgumentException("Could not find any client_secret.json file in " + path);

            var serializer = new JsonSerializer();

            using (var sr = new StreamReader(path))
            using (var reader = new JsonTextReader(sr))
            {
                var datamodel = serializer.Deserialize<ClientSecretModel>(reader);
                return datamodel;
            }
        }
    }
}
