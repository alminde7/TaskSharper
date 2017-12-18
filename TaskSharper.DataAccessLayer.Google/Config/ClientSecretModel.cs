using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TaskSharper.DataAccessLayer.Google.Config
{
    /// <summary>
    /// Model for client secret provided by Google Calendar. Name of properties is in correspondance with the naming in the file.
    /// </summary>
    internal class ClientSecretModel
    {
        public Installed installed { get; set; }

        public class Installed
        {
            public string client_id { get; set; }
            public string project_id { get; set; }
            public string auth_uri { get; set; }
            public string token_uri { get; set; }
            public string auth_provider_x509_cert_url { get; set; }
            public string client_secret { get; set; }
            public List<string> redirect_uris { get; set; }
        }
    }
}
