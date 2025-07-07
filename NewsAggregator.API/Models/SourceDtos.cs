namespace NewsAggregator.API.Models
{
    public class SourceRequest
    {
        public string ApiName { get; set; }
        public string BaseUrl { get; set; }
        public string ApiKey { get; set; }
    }
    public class SourceUpdateRequest
    {
        public string? ApiName { get; set; }
        public string? BaseUrl { get; set; }
        public string? ApiKey { get; set; }
    }

}
