using Microsoft.EntityFrameworkCore;
using NewsAggregator.DAL.Context;
using NewsAggregator.DAL.Entities;

namespace NewsAggregator.DAL.Repository
{
    public interface INotificationRepository
    {
        Task<List<Notification>> GetUserNotificationsAsync(Guid userId);
        Task<List<string>> GetUserEnabledCategoriesAsync(Guid userId);
        Task<List<string>> GetUserKeywordsAsync(Guid userId);
        Task<bool> SetCategoryNotificationAsync(Guid userId, string category, bool enabled);
        Task<bool> SetKeywordNotificationsAsync(Guid userId, List<string> keywords);
        Task AddCategoryAndKeywordNotificationsForArticlesAsync(List<Article> articles);
    }

    public class NotificationRepository : INotificationRepository
    {
        private readonly NewsAggregatorDbContext _dbContext;

        public NotificationRepository(NewsAggregatorDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Notification>> GetUserNotificationsAsync(Guid userId)
        {
            var notifications = await _dbContext.Notifications
                .Include(n => n.Article)
                .Include(n => n.Article.Source)
                .Include(n => n.Article.Category)
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.SentAt)
                .ToListAsync();

            var mappedNotifications = notifications.Select(n => new Notification
            {
                Id = n.Id,
                UserId = n.UserId,
                ArticleId = n.ArticleId,
                SentAt = n.SentAt,
                Type = n.Type,
                Article = new Article
                {
                    Id = n.Article.Id,
                    Title = n.Article.Title,
                    Url = n.Article.Url,
                    Source = new Source
                    {
                        Name = n.Article.Source.Name
                    },
                    Category = new Category
                    {
                        Name = n.Article.Category.Name
                    }
                    
                }
            }).ToList();

            return mappedNotifications;
        }

        public async Task<List<string>> GetUserEnabledCategoriesAsync(Guid userId)
        {
            return await _dbContext.NotificationPreferences
                .Where(np => np.UserId == userId && np.Enabled)
                .Select(np => np.Category.Name)
                .ToListAsync();
        }

        public async Task<List<string>> GetUserKeywordsAsync(Guid userId)
        {
            return await _dbContext.NotificationKeywords
                .Where(nk => nk.UserId == userId)
                .Select(nk => nk.Keyword)
                .ToListAsync();
        }

        public async Task<bool> SetCategoryNotificationAsync(Guid userId, string category, bool enabled)
        {
            var cat = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Name == category.ToLower());
            if (cat == null) return false;

            var pref = await _dbContext.NotificationPreferences
                .FirstOrDefaultAsync(np => np.UserId == userId && np.CategoryId == cat.Id);

            if (pref == null)
            {
                pref = new NotificationPreference
                {
                    UserId = userId,
                    CategoryId = cat.Id,
                    Enabled = enabled
                };
                _dbContext.NotificationPreferences.Add(pref);
            }
            else
            {
                pref.Enabled = enabled;
                _dbContext.NotificationPreferences.Update(pref);
            }

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SetKeywordNotificationsAsync(Guid userId, List<string> keywords)
        {
            var existing = _dbContext.NotificationKeywords.Where(nk => nk.UserId == userId);
            _dbContext.NotificationKeywords.RemoveRange(existing);

            foreach (var keyword in keywords.Distinct())
            {
                _dbContext.NotificationKeywords.Add(new NotificationKeyword
                {
                    UserId = userId,
                    Keyword = keyword
                });
            }

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task AddCategoryAndKeywordNotificationsForArticlesAsync(List<Article> articles)
        {
            foreach (var article in articles)
            {
                var categoryUserIds = await _dbContext.NotificationPreferences
                    .Where(np => np.CategoryId == article.CategoryId && np.Enabled)
                    .Select(np => np.UserId)
                    .ToListAsync();

                var categoryNotifications = categoryUserIds.Select(userId => new Notification
                {
                    UserId = userId,
                    ArticleId = article.Id,
                    SentAt = DateTime.UtcNow,
                    Type = "Category"
                }).ToList();

                var keywordUserIds = await _dbContext.NotificationKeywords
                    .Where(nk =>
                        article.Title.ToLower().Contains(nk.Keyword.ToLower()) ||
                        article.Content.ToLower().Contains(nk.Keyword.ToLower()))
                    .Select(nk => nk.UserId)
                    .Distinct()
                    .ToListAsync();

                var keywordNotifications = keywordUserIds
                    .Where(userId => !categoryUserIds.Contains(userId))
                    .Select(userId => new Notification
                    {
                        UserId = userId,
                        ArticleId = article.Id,
                        SentAt = DateTime.UtcNow,
                        Type = "Keyword"
                    }).ToList();

                var allNotifications = categoryNotifications.Concat(keywordNotifications).ToList();

                if (allNotifications.Count > 0)
                {
                    _dbContext.Notifications.AddRange(allNotifications);
                    await _dbContext.SaveChangesAsync();
                }
            }
        }
    }
}