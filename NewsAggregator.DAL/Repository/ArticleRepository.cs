using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NewsAggregator.DAL.Context;
using NewsAggregator.DAL.Entities;

namespace NewsAggregator.DAL.Repository
{
    public interface IArticleRepository
    {
        Task SaveArticlesAsync(IEnumerable<Article> articles);
        Task<int> CategorizeUnknownArticlesByKeywordsAsync();
        Task<List<Article>> GetArticlesByDateRangeAsync(DateOnly start, DateOnly end);
        Task<List<Article>> GetArticlesByCategoryAndDateAsync(string category, DateTime date);
        Task<List<Article>> GetArticlesByCategoryAndDateRangeAsync(string category, DateOnly start, DateOnly end);
        Task<List<Article>> SearchArticlesAsync(string query, DateOnly? start, DateOnly? end, string? sortBy);
        Task<bool> SaveArticleForUserAsync(int articleId, Guid userId);
        Task<bool> DeleteSavedArticleForUserAsync(int articleId, Guid userId);
        Task<List<Article>> GetSavedArticlesForUserAsync(Guid userId);
        Task<bool> SetArticleFeedbackAsync(int articleId, Guid userId, bool? isLike, bool? isReported);
        Task<Article?> GetArticleByIdAsync(int articleId);
        Task<int> HideArticlesWithHiddenKeywordsAsync(IEnumerable<string> hiddenKeywords);
        Task<List<Article>> GetAllReportedNotHiddenArticlesAsync();
        Task HideArticlesAsync(int id);
    }

    public class ArticleRepository(NewsAggregatorDbContext context) : IArticleRepository
    {
        private const int ARTICLE_REPORT_THRESHOLD = 10;
        public async Task<int> CategorizeUnknownArticlesByKeywordsAsync()
        {
            var unknownCategory = await context.Categories
                .FirstOrDefaultAsync(c => c.Name.ToLower() == "unknown");
            if (unknownCategory == null) return 0;

            var keywords = await context.CategoryKeywords.Include(ck => ck.Category).ToListAsync();
            var articles = await context.Articles
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
                await context.SaveChangesAsync();

            return updated;
        }

        public async Task SaveArticlesAsync(IEnumerable<Article> articles)
        {
            foreach (var article in articles)
            {
                if (!context.Articles.Any(a => a.Title == article.Title))
                    context.Articles.Add(article);
            }
            await context.SaveChangesAsync();
        }

        public async Task<List<Article>> GetArticlesByDateRangeAsync(DateOnly start, DateOnly end)
        {
            var startDateTime = start.ToDateTime(TimeOnly.MinValue);
            var endDateTime = end.ToDateTime(TimeOnly.MaxValue);
            return await context.Articles
                .Where(a => a.PublishedAt >= startDateTime && a.PublishedAt <= endDateTime && !a.IsHidden && !a.Category.IsHidden)
                .ToListAsync();
        }

        public async Task<List<Article>> GetArticlesByCategoryAndDateAsync(string category, DateTime date)
        {
            return await context.Articles
                .Include(a => a.Category)
                .Where(a => a.Category.Name == category && a.PublishedAt >= date.Date && a.PublishedAt < date.Date.AddDays(1) && !a.IsHidden && !a.Category.IsHidden)
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

        public async Task<List<Article>> GetArticlesByCategoryAndDateRangeAsync(string category, DateOnly start, DateOnly end)
        {
            var startDateTime = start.ToDateTime(TimeOnly.MinValue);
            var endDateTime = end.ToDateTime(TimeOnly.MaxValue);

            return await context.Articles
                .Include(a => a.Category)
                .Where(a =>
                    a.Category.Name == category &&
                    a.PublishedAt >= startDateTime &&
                    a.PublishedAt <= endDateTime &&
                    !a.IsHidden && 
                    !a.Category.IsHidden)
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
                    },
                    Likes = a.Likes,
                    Dislikes = a.Dislikes
                })
                .ToListAsync();
        }

        public async Task<List<Article>> SearchArticlesAsync(string query, DateOnly? start, DateOnly? end, string? sortBy)
        {
            var articlesQuery = context.Articles.AsQueryable();

            if (!string.IsNullOrEmpty(query))
            {
                articlesQuery = articlesQuery.Where(a =>
                    a.Title.ToLower().Contains(query.ToLower()) ||
                    a.Content.ToLower().Contains(query.ToLower()));
            }

            if (start.HasValue)
            {
                var startDateTime = start.Value.ToDateTime(TimeOnly.MinValue);
                articlesQuery = articlesQuery.Where(a => a.PublishedAt >= startDateTime);
            }

            if (end.HasValue)
            {
                var endDateTime = end.Value.ToDateTime(TimeOnly.MaxValue);
                articlesQuery = articlesQuery.Where(a => a.PublishedAt <= endDateTime);
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

            return await articlesQuery.Where(a => !a.IsHidden && !a.Category.IsHidden).ToListAsync();
        }

        public async Task<bool> SaveArticleForUserAsync(int articleId, Guid userId)
        {
            var alreadySaved = await context.SavedArticles
                .AnyAsync(sa => sa.ArticleId == articleId && sa.UserId == userId);
            if (!alreadySaved)
            {
                context.SavedArticles.Add(new SavedArticle
                {
                    ArticleId = articleId,
                    UserId = userId,
                    SavedAt = DateTime.UtcNow
                });
                await context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteSavedArticleForUserAsync(int articleId, Guid userId)
        {
            var saved = await context.SavedArticles
                .FirstOrDefaultAsync(sa => sa.ArticleId == articleId && sa.UserId == userId);
            if (saved != null)
            {
                context.SavedArticles.Remove(saved);
                await context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<Article>> GetSavedArticlesForUserAsync(Guid userId)
        {
            return await context.SavedArticles
                .Include(sa => sa.Article)
                .Where(sa => sa.UserId == userId && !sa.Article.IsHidden && !sa.Article.Category.IsHidden)
                .Select(sa => sa.Article)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> SetArticleFeedbackAsync(int articleId, Guid userId, bool? isLike, bool? isReported)
        {
            var article = await context.Articles.FirstOrDefaultAsync(a => a.Id == articleId && !a.IsHidden && !a.Category.IsHidden);
            if (article == null) return false;

            var feedback = await context.ArticleFeedbacks
                .FirstOrDefaultAsync(f => f.ArticleId == articleId && f.UserId == userId);

            if (feedback != null)
            {
                if (isLike.HasValue)
                {
                    if (feedback.IsLike.HasValue && feedback.IsLike.Value == isLike.Value) return true;
                    if (isLike.Value)
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
                if (isReported.HasValue && isReported.Value == true)
                {
                    if (!feedback.IsReported)
                    {
                        article.ReportCount++;
                        if (article.ReportCount >= ARTICLE_REPORT_THRESHOLD)
                        {
                            article.IsHidden = true;
                        }
                        feedback.IsReported = true;
                    }
                }

            }
            else
            {
                context.ArticleFeedbacks.Add(new ArticleFeedback
                {
                    ArticleId = articleId,
                    UserId = userId,
                    IsLike = isLike??false,
                    IsReported = isReported ?? false,
                    CreatedAt = DateTime.UtcNow
                });
                if (isLike.HasValue)
                {
                    if (isLike.Value)
                        article.Likes++;
                    else
                        article.Dislikes++;
                }
               
                article.ReportCount++;
            }

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<Article?> GetArticleByIdAsync(int articleId)
        {
            var result = await context.Articles
                .Where(a => a.Id == articleId && !a.IsHidden && !a.Category.IsHidden)
                .Select(a => new
                {
                    a.Id,
                    a.Title,
                    a.Url,
                    a.Content,
                    a.PublishedAt,
                    a.SourceId,
                    Source = a.Source == null ? null : new
                    {
                        a.Source.Id,
                        a.Source.Name,
                        a.Source.ApiKey,
                        a.Source.ApiUrl,
                        a.Source.LastAccessedDate
                    },
                    a.CategoryId,
                    Category = a.Category == null ? null : new
                    {
                        a.Category.Id,
                        a.Category.Name
                    },
                    a.Likes,
                    a.Dislikes,
                    a.IsHidden,
                    a.ReportCount
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (result == null) return null;

            return new Article
            {
                Id = result.Id,
                Title = result.Title,
                Url = result.Url,
                Content = result.Content,
                PublishedAt = result.PublishedAt,
                SourceId = result.SourceId,
                Source = new Source
                {
                    Id = result.Source!.Id,
                    Name = result.Source.Name,
                },
                CategoryId = result.CategoryId,
                Category = new Category
                {
                    Id = result.Category!.Id,
                    Name = result.Category.Name
                },
                Likes = result.Likes,
                Dislikes = result.Dislikes
            };
        }

        public async Task<int> HideArticlesWithHiddenKeywordsAsync(IEnumerable<string> hiddenKeywords)
        {
            var articles = await context.Articles.ToListAsync();
            int updatedCount = 0;
            foreach (var article in articles)
            {
                foreach (var keyword in hiddenKeywords)
                {
                    if (article.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                        article.Content.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    {
                        article.IsHidden = true;
                        updatedCount++;
                        break;
                    }
                }
            }
            if (updatedCount > 0)
                await context.SaveChangesAsync();
            return updatedCount;
        }

        public async Task<List<Article>> GetAllReportedNotHiddenArticlesAsync()
        {
            return await context.Articles
                .Where(a => a.ReportCount > 0 && !a.IsHidden && !a.Category.IsHidden)
                .ToListAsync();
        }
        public async Task HideArticlesAsync(int id)
        {
            var article = await context.Articles.FindAsync(id);
            if (article != null)
            {
                article.IsHidden = true;
                await context.SaveChangesAsync();
            }
        }
    }
}