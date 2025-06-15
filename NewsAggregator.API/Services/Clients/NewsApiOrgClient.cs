using NewsAggregator.API.Models;
using NewsAggregator.DAL.Entities;
using NewsAggregator.DAL.Repository;
using System.Text.Json;

namespace NewsAggregator.API.Services.Clients
{
    public class NewsApiOrgClient(HttpClient httpClient, string apiUrl, string apiKey, ICategoryRepository categoryRepository, ISourceRepository sourceRepository) : INewsApiClient
    {
        async Task<List<Article>> INewsApiClient.FetchArticlesAsync()
        {
            var url = $"{apiUrl}&apiKey={apiKey}";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("User-Agent", "Mozilla/5.0 (compatible; NewsAggregator/1.0)");
            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<NewsApiOrgResponse>(json, options);

            if (result != null && result.Articles != null && result.Status == "ok" && result.TotalResults > 0)
            {
                var category = await categoryRepository.GetByIdAsync(5);
                var source = await sourceRepository.GetByNameAsync("NewsApiOrg");

                var mappedArticles = result.Articles.Select(a => new Article
                {
                    Title = a.Title,
                    Url = a.Url,
                    Content = a.Content ?? a.Description ?? "",
                    PublishedAt = DateTime.TryParse(a.PublishedAt.ToString(), out var dt) ? dt : DateTime.UtcNow,
                    SourceId = source.Id,
                    //Source = source,
                    CategoryId = category.Id
                    //Category = category
                });
                return [.. mappedArticles];
            }
            return [];
        }
    }
}