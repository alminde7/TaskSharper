using System;
using System.IO;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Calendar.v3;
using Google.Apis.Util.Store;
using Newtonsoft.Json;
using Serilog;
using TaskSharper.DataAccessLayer.Google.Config;

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

        // TODO:: This method is not threadsafe. Maybe load the client token into memory and only update it if request fails?
        public UserCredential Authenticate()
        {
            UserCredential credential;

            var pathToSecret = Path.Combine(Shared.Configuration.Config.TaskSharperCredentialStore, "client_secret.json");
            if (!File.Exists(pathToSecret)) throw new ArgumentException("Could not find any client_secret.json file in " + pathToSecret);

            var model = LoadClientSecret(pathToSecret);
            
            var credPath = Path.Combine(Shared.Configuration.Config.TaskSharperCredentialStore, "calendar.json");

            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets()
                {
                    ClientId = model.installed.client_id,
                    ClientSecret = model.installed.client_secret
                }, 
                _scopes,
                "user",
                CancellationToken.None,
                new FileDataStore(credPath, true)).Result;


            Logger.Information($"Credential file saved to: {credPath}");

            return credential;
        }

        // NOTE:: I am not quite sure about this ?
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
