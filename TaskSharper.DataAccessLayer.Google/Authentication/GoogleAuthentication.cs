    using System;
using System.IO;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Util.Store;
using Serilog;
    using TaskSharper.Shared.Configuration;
    using TaskSharper.Shared.Logging;

namespace TaskSharper.DataAccessLayer.Google.Authentication
{
    public class GoogleAuthentication
    {
        protected ILogger Logger;
        private readonly string[] _scopes = { CalendarService.Scope.Calendar };

        public GoogleAuthentication(ILogger logger)
        {
            Logger = logger;
        }
        public UserCredential Authenticate()
        {
            UserCredential credential;

            var pathToSecret = Path.Combine(Config.TaskSharperCredentialStore, "client_secret.json");
            if(!File.Exists(pathToSecret)) throw new ArgumentException("Could not find any client_secret.json file in " + pathToSecret);

            using (var stream = new FileStream(Path.Combine(pathToSecret), FileMode.Open, FileAccess.Read))
            {
                string credPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                credPath = Path.Combine(Config.TaskSharperCredentialStore, "calendar.json");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    _scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;

                Logger.Information($"Credential file saved to: {credPath}");
            }

            return credential;
        }
    }
}
