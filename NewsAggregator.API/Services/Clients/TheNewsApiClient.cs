using NewsAggregator.API.Models;
using NewsAggregator.DAL.Entities;
using NewsAggregator.DAL.Repository;
using System.Text.Json;

namespace NewsAggregator.API.Services.Clients
{
    public class TheNewsApiClient(HttpClient httpClient, Source source, ICategoryRepository categoryRepository) : INewsApiClient
    {
        async Task<List<Article>> INewsApiClient.FetchArticlesAsync()
        {
            var url = $"{source.ApiUrl} &api_token= {source.ApiKey}";
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<TheNewsApiResponse>(json, options);

            if (result != null && result.Data != null && result.Data.Count>0 && result.Meta.Returned == result.Data.Count)
            {
                var categories = await categoryRepository.GetAllAsync();
                var defaultCategory = await categoryRepository.GetByIdAsync(1);
                

                var mappedArticles = result.Data.Select(a => {
                    int categoryId = defaultCategory.Id;
                    if (a.Categories != null && a.Categories.Count > 0)
                    {
                        var match = categories.FirstOrDefault(c =>
                            a.Categories.Any(apiCat => string.Equals(apiCat, c.Name, StringComparison.OrdinalIgnoreCase)));
                        if (match != null)
                            categoryId = match.Id;
                    }
                    return new Article
                    {
                        Title = a.Title,
                        Url = a.Url,
                        Content = a.Description ?? "",
                        PublishedAt = DateTime.TryParse(a.PublishedAt.ToString(), out var dt) ? dt : DateTime.UtcNow,
                        SourceId = source.Id,
                        CategoryId = categoryId,
                    };
                });
                return [.. mappedArticles];
            }
            return [];
        }
    }
}