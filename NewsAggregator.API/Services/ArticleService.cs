using NewsAggregator.DAL.Entities;
using NewsAggregator.DAL.Repository;

namespace NewsAggregator.API.Services
{
    public interface IArticleService
    {
        Task<bool> SaveArticleAsync(int articleId, Guid userId);
        Task<List<Article>> GetSavedArticlesAsync(Guid userId);
        Task<bool> DeleteSavedArticleAsync(int articleId, Guid userId);
        Task<bool> SetArticleFeedbackAsync(int articleId, Guid userId, bool isLike);
        Task<Article?> GetArticleByIdAsync(int articleId);
    }

    public class ArticleService(IArticleRepository articleRepository) : IArticleService
    {
        public async Task<bool> SaveArticleAsync(int articleId, Guid userId)
            => await articleRepository.SaveArticleForUserAsync(articleId, userId);
        
        public async Task<List<Article>> GetSavedArticlesAsync(Guid userId)
            => await articleRepository.GetSavedArticlesForUserAsync(userId);

        public async Task<bool> DeleteSavedArticleAsync(int articleId, Guid userId)
            => await articleRepository.DeleteSavedArticleForUserAsync(articleId, userId);
        
        public async Task<bool> SetArticleFeedbackAsync(int articleId, Guid userId, bool isLike)
            => await articleRepository.SetArticleFeedbackAsync(articleId, userId, isLike);

        public async Task<Article?> GetArticleByIdAsync(int articleId)
            => await articleRepository.GetArticleByIdAsync(articleId);
    }
}