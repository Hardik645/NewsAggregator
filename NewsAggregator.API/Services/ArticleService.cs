using NewsAggregator.DAL.Entities;
using NewsAggregator.DAL.Repository;

namespace NewsAggregator.API.Services
{
    public interface IArticleService
    {
        Task<bool> SaveArticleAsync(int articleId, Guid userId);
        Task<List<Article>> GetSavedArticlesAsync(Guid userId);
        Task<bool> DeleteSavedArticleAsync(int articleId, Guid userId);
        Task<bool> SetArticleFeedbackAsync(int articleId, Guid userId, bool? isLike, bool? isReported);
        Task<Article?> GetArticleByIdAsync(int articleId);
        Task<List<Article>> GetAllReportedNotHiddenArticlesAsync();
        Task HideArticlesAsync(int id);
    }

    public class ArticleService(IArticleRepository articleRepository) : IArticleService
    {
        public async Task<bool> SaveArticleAsync(int articleId, Guid userId)
            => await articleRepository.SaveArticleForUserAsync(articleId, userId);
        
        public async Task<List<Article>> GetSavedArticlesAsync(Guid userId)
            => await articleRepository.GetSavedArticlesForUserAsync(userId);

        public async Task<bool> DeleteSavedArticleAsync(int articleId, Guid userId)
            => await articleRepository.DeleteSavedArticleForUserAsync(articleId, userId);
        
        public async Task<bool> SetArticleFeedbackAsync(int articleId, Guid userId, bool? isLike, bool? isReported)
            => await articleRepository.SetArticleFeedbackAsync(articleId, userId, isLike, isReported);

        public async Task<Article?> GetArticleByIdAsync(int articleId)
            => await articleRepository.GetArticleByIdAsync(articleId);

        public async Task<List<Article>> GetAllReportedNotHiddenArticlesAsync()
            => await articleRepository.GetAllReportedNotHiddenArticlesAsync();

        public async Task HideArticlesAsync(int id)
            => await articleRepository.HideArticlesAsync(id);
    }
}