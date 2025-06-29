using NewsAggregator.DAL.Repository;
using NewsAggregator.DAL.Entities;

namespace NewsAggregator.API.Services
{
    public interface INewsService
    {
        Task<List<Article>> GetTodaysNewsAsync(string? category);
        Task<List<Article>> GetNewsByDateRangeAsync(DateOnly startDate, DateOnly endDate, string? category);
        Task<List<Article>> GetTodaysNewsByCategoryAsync(string category);
        Task<List<Article>> SearchNewsAsync(string query, DateTime? startDate, DateTime? endDate, string? sortBy);
    }

    public class NewsService(IArticleRepository articleRepository) : INewsService
    {
        public async Task<List<Article>> GetTodaysNewsAsync(string? category)
        {
            try
            {
                var today = DateTime.UtcNow.Date;
                if (string.IsNullOrEmpty(category) || string.Equals(category, "all", StringComparison.OrdinalIgnoreCase))
                {
                    var todayStartTime = DateOnly.FromDateTime(today);
                    var todayEndTime = DateOnly.FromDateTime(today.AddDays(1));
                    return await articleRepository.GetArticlesByDateRangeAsync(todayStartTime, todayEndTime);
                }
                else
                {
                    return await articleRepository.GetArticlesByCategoryAndDateAsync(category, today);
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to get today's news.", ex);
            }
        }

        public async Task<List<Article>> GetNewsByDateRangeAsync(DateOnly startDate, DateOnly endDate, string? category)
        {
            try
            {
                if (string.IsNullOrEmpty(category) || string.Equals(category, "all", StringComparison.OrdinalIgnoreCase))
                {
                    return await articleRepository.GetArticlesByDateRangeAsync(startDate, endDate);
                }
                else
                {
                    var startDateTime = startDate.ToDateTime(TimeOnly.MinValue);
                    var endDateTime = endDate.ToDateTime(TimeOnly.MaxValue);

                    return await articleRepository.GetArticlesByCategoryAndDateRangeAsync(category,startDate, endDate);
                }
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