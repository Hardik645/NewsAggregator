using NewsAggregator.DAL.Entities;

namespace NewsAggregator.API.Services.Clients
{
    public class NewsApiClientFactory(IServiceProvider serviceProvider, HttpClient httpClient)
    {
        public INewsApiClient GetClient(Source source)
        {
            var scopedProvider = serviceProvider.CreateScope().ServiceProvider;
            return source.Name switch
            {
                "NewsApiOrg" => ActivatorUtilities.CreateInstance<NewsApiOrgClient>(scopedProvider, httpClient, source.ApiUrl, source.ApiKey),
                "TheNewsApi" => ActivatorUtilities.CreateInstance<TheNewsApiClient>(scopedProvider, httpClient, source.ApiUrl, source.ApiKey),
                _ => throw new ArgumentException("Unknown news source")
            };
        }
    }
}