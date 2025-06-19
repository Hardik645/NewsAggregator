using NewsAggregator.DAL.Entities;
using NewsAggregator.DAL.Repository;
using System.Security.Claims;

namespace NewsAggregator.API.Services
{
    public interface IArticleService
    {
        Task<bool> SaveArticleAsync(int articleId, Guid userId);
        Task<List<Article>> GetSavedArticlesAsync(Guid userId);
        Task<bool> DeleteSavedArticleAsync(int articleId, Guid userId);
    }

    public class ArticleService : IArticleService
    {
        private readonly IArticleRepository _articleRepository;

        public ArticleService(IArticleRepository articleRepository)
        {
            _articleRepository = articleRepository;
        }

        public async Task<bool> SaveArticleAsync(int articleId, Guid userId)
        {
            return await _articleRepository.SaveArticleForUserAsync(articleId, userId);
        }

        public async Task<List<Article>> GetSavedArticlesAsync(Guid userId)
        {
            return await _articleRepository.GetSavedArticlesForUserAsync(userId);
        }

        public async Task<bool> DeleteSavedArticleAsync(int articleId, Guid userId)
        {
            return await _articleRepository.DeleteSavedArticleForUserAsync(articleId, userId);
        }
    }
}