using NewsAggregator.API.Models;
using NewsAggregator.DAL.Entities;

namespace NewsAggregator.API.Services.Clients
{
    public interface INewsApiClient
    {
        Task<List<Article>> FetchArticlesAsync();
    }
}