using System;
using System.Diagnostics;
using System.Net.Http;

namespace TaskSharper.Shared.Helpers
{
    public class ConnectionHelper
    {
        public static bool CheckConnectionElasticsearch(string url)
        {
            try
            {
                var httpClient = new HttpClient();
                var result = httpClient.GetAsync(url).Result;
                return result.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
                return false;
            }
        }
    }
}
