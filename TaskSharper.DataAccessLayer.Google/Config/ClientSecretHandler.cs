using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TaskSharper.DataAccessLayer.Google.Config
{
    public class ClientSecretHandler
    {
        public void LoadClientSecret(string path)
        {
            if (!File.Exists(path))
                throw new ArgumentException("Could not find any client_secret.json file in " + path);

            var serializer = new JsonSerializer();

            var model = new ClientSecretModel();
            using (var sr = new StreamReader(path))
            using (var reader = new JsonTextReader(sr))
            {
                var data = serializer.Deserialize(reader);
                
            }

        }
    }
}
