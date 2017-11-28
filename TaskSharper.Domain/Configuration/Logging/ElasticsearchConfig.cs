namespace TaskSharper.Domain.Configuration.Logging
{
    public class ElasticsearchConfig
    {
        public string Url { get; set; } = "http://alminde1.mynetgear.com:9200";
        public bool EnableAuthentication { get; set; } = false;
        public string Username { get; set; }
        public string Password { get; set; }
    }
}