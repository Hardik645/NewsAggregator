using NewsAggregator.DAL.Repository;
using NewsAggregator.DAL.Entities;

namespace NewsAggregator.API.Services
{
    public interface INewsService
    {
        Task<List<Article>> GetTodaysNewsAsync();
        Task<List<Article>> GetNewsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<List<Article>> GetTodaysNewsByCategoryAsync(string category);
        Task<List<Article>> SearchNewsAsync(string query, DateTime? startDate, DateTime? endDate, string? sortBy);
    }

    public class NewsService(IArticleRepository articleRepository) : INewsService
    {
        public async Task<List<Article>> GetTodaysNewsAsync()
        {
            try
            {
                var todayStartTime = DateTime.UtcNow.Date;
                var todayEndTime = DateTime.UtcNow.Date.Add(TimeSpan.FromDays(1));
                return await articleRepository.GetArticlesByDateRangeAsync(todayStartTime, todayEndTime);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to get today's news.", ex);
            }
        }

        public async Task<List<Article>> GetNewsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                return await articleRepository.GetArticlesByDateRangeAsync(startDate.Date, endDate.Date);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to get news by date range.", ex);
            }
        }

        public async Task<List<Article>> GetTodaysNewsByCategoryAsync(string category)
        {
            try
            {
                var today = DateTime.UtcNow.Date;
                return await articleRepository.GetArticlesByCategoryAndDateAsync(category, today);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to get today's news by category.", ex);
            }
        }

        public async Task<List<Article>> SearchNewsAsync(string query, DateTime? startDate, DateTime? endDate, string? sortBy)
        {
            try
            {
                return await articleRepository.SearchArticlesAsync(query, startDate, endDate, sortBy);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to search news.", ex);
            }
        }
    }
}