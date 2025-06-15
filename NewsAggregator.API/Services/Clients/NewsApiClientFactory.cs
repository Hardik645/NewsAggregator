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
                "NewsApiOrg" => ActivatorUtilities.CreateInstance<NewsApiOrgClient>(scopedProvider, httpClient, source),
                "TheNewsApi" => ActivatorUtilities.CreateInstance<TheNewsApiClient>(scopedProvider, httpClient, source),
                _ => throw new ArgumentException("Unknown news source")
            };
        }
    }
}