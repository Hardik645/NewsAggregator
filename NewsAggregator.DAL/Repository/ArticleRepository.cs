using Microsoft.EntityFrameworkCore;
using NewsAggregator.DAL.Context;
using NewsAggregator.DAL.Entities;

namespace NewsAggregator.DAL.Repository
{
    public interface IArticleRepository
    {
        Task SaveArticlesAsync(IEnumerable<Article> articles);
        Task<int> CategorizeUnknownArticlesByKeywordsAsync();
        Task<List<Article>> GetArticlesByDateRangeAsync(DateTime start, DateTime end);
        Task<List<Article>> GetArticlesByCategoryAndDateAsync(string category, DateTime date);
        Task<List<Article>> SearchArticlesAsync(string query, DateTime? start, DateTime? end, string? sortBy);
        Task<bool> SaveArticleForUserAsync(int articleId, Guid userId);
        Task<bool> DeleteSavedArticleForUserAsync(int articleId, Guid userId);
        Task<List<Article>> GetSavedArticlesForUserAsync(Guid userId);
        Task<bool> SetArticleFeedbackAsync(int articleId, Guid userId, bool isLike);
    }

    public class ArticleRepository : IArticleRepository
    {
        private readonly NewsAggregatorDbContext _context;

        public ArticleRepository(NewsAggregatorDbContext context)
        {
            _context = context;
        }

        public async Task<int> CategorizeUnknownArticlesByKeywordsAsync()
        {
            var unknownCategory = await _context.Categories
                .FirstOrDefaultAsync(c => c.Name.ToLower() == "unknown"); 
            if (unknownCategory == null) return 0;

            var keywords = await _context.CategoryKeywords.Include(ck => ck.Category).ToListAsync();
            var articles = await _context.Articles
                .Where(a => a.CategoryId == unknownCategory.Id)
                .ToListAsync();

            int updated = 0;
            foreach (var article in articles)
            {
                var match = keywords.FirstOrDefault(ck =>
                    article.Title.Contains(ck.Keyword, StringComparison.OrdinalIgnoreCase) ||
                    article.Content.Contains(ck.Keyword, StringComparison.OrdinalIgnoreCase));

                if (match != null)
                {
                    article.CategoryId = match.CategoryId;
                    updated++;
                }
            }

            if (updated > 0)
                await _context.SaveChangesAsync();

            return updated;
        }

        public async Task SaveArticlesAsync(IEnumerable<Article> articles)
        {
            foreach (var article in articles)
            {
                if (!_context.Articles.Any(a => a.Title == article.Title))
                    _context.Articles.Add(article);
            }
            await _context.SaveChangesAsync();
        }

        public async Task<List<Article>> GetArticlesByDateRangeAsync(DateTime start, DateTime end)
        {
            return await _context.Articles
                .Where(a => a.PublishedAt >= start && a.PublishedAt <= end)
                .ToListAsync();
        }

        public async Task<List<Article>> GetArticlesByCategoryAndDateAsync(string category, DateTime date)
        {
            return await _context.Articles
                .Include(a => a.Category)
                .Where(a => a.Category.Name == category && a.PublishedAt >= date.Date && a.PublishedAt < date.Date.AddDays(1))
                .Select(a => new Article
                {
                    Id = a.Id,
                    Title = a.Title,
                    Url = a.Url,
                    Content = a.Content,
                    PublishedAt = a.PublishedAt,
                    SourceId = a.SourceId,
                    Source = a.Source,
                    CategoryId = a.CategoryId,
                    Category = new Category
                    {
                        Id = a.Category.Id,
                        Name = a.Category.Name
                    }
                })
                .ToListAsync();
        }

        public async Task<List<Article>> SearchArticlesAsync(string query, DateTime? start, DateTime? end, string? sortBy)
        {
            var articlesQuery = _context.Articles.AsQueryable();

            if (!string.IsNullOrEmpty(query))
            {
                articlesQuery = articlesQuery.Where(a =>
                    a.Title.ToLower().Contains(query.ToLower()) ||
                    a.Content.ToLower().Contains(query.ToLower()));
            }

            if (start.HasValue)
            {
                articlesQuery = articlesQuery.Where(a => a.PublishedAt >= start.Value);
            }

            if (end.HasValue)
            {
                articlesQuery = articlesQuery.Where(a => a.PublishedAt <= end.Value);
            }

            if (!string.IsNullOrEmpty(sortBy))
            {
                articlesQuery = sortBy.ToLower() switch
                {
                    "title" => articlesQuery.OrderBy(a => a.Title),
                    "date" => articlesQuery.OrderBy(a => a.PublishedAt),
                    _ => articlesQuery
                };
            }

            return await articlesQuery.ToListAsync();
        }

        public async Task<bool> SaveArticleForUserAsync(int articleId, Guid userId)
        {
            var alreadySaved = await _context.SavedArticles
                .AnyAsync(sa => sa.ArticleId == articleId && sa.UserId == userId);
            if (!alreadySaved)
            {
                _context.SavedArticles.Add(new SavedArticle
                {
                    ArticleId = articleId,
                    UserId = userId,
                    SavedAt = DateTime.UtcNow
                });
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteSavedArticleForUserAsync(int articleId, Guid userId)
        {
            var saved = await _context.SavedArticles
                .FirstOrDefaultAsync(sa => sa.ArticleId == articleId && sa.UserId == userId);
            if (saved != null)
            {
                _context.SavedArticles.Remove(saved);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<Article>> GetSavedArticlesForUserAsync(Guid userId)
        {
            return await _context.SavedArticles
                .Where(sa => sa.UserId == userId)
                .Include(sa => sa.Article)
                .Select(sa => sa.Article)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> SetArticleFeedbackAsync(int articleId, Guid userId, bool isLike)
        {
            var article = await _context.Articles.FirstOrDefaultAsync(a => a.Id == articleId);
            if (article == null) return false;

            var feedback = await _context.ArticleFeedbacks
                .FirstOrDefaultAsync(f => f.ArticleId == articleId && f.UserId == userId);

            if (feedback != null)
            {
                if (feedback.IsLike == isLike) return true;

                if (isLike)
                {
                    feedback.IsLike = true;
                    article.Likes++;
                    article.Dislikes = Math.Max(0, article.Dislikes - 1);
                }
                else
                {
                    feedback.IsLike = false;
                    article.Dislikes++;
                    article.Likes = Math.Max(0, article.Likes - 1);
                }
            }
            else
            {
                _context.ArticleFeedbacks.Add(new ArticleFeedback
                {
                    ArticleId = articleId,
                    UserId = userId,
                    IsLike = isLike,
                    CreatedAt = DateTime.UtcNow
                });
                if (isLike)
                    article.Likes++;
                else
                    article.Dislikes++;
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}