using System.Net.Http;

namespace TaskSharper.Shared.Helpers
{
    public class ConnectionHelper
    {
        public static bool CheckConnectionElasticsearch(string url)
        {
            var httpClient = new HttpClient();
            var result = httpClient.GetAsync(url).Result;
            return result.IsSuccessStatusCode;
        }
    }
}
