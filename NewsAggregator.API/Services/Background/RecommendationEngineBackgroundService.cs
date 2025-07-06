using Microsoft.EntityFrameworkCore;
using NewsAggregator.DAL.Context;
using NewsAggregator.DAL.Entities;

namespace NewsAggregator.API.Services.Background
{
    public class RecommendationEngineBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<RecommendationEngineBackgroundService> logger) : BackgroundService
    {
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(5);

        private static readonly Dictionary<string, int> ActionPoints = new()
        {
            { "view", 1 },
            { "like", 5 },
            { "save", 10 }
        };

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("User action processing service started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = serviceProvider.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<NewsAggregatorDbContext>();

                    await ProcessLogsAsync(dbContext, stoppingToken);                   
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error occurred while processing user action logs.");
                }

                await Task.Delay(_interval, stoppingToken);
            }

            logger.LogInformation("User action processing service stopped.");
        }

        private async Task ProcessLogsAsync(NewsAggregatorDbContext dbContext, CancellationToken cancellationToken)
        {
            var unprocessedLogs = await GetUnprocessedLogsAsync(dbContext, cancellationToken);

            if (unprocessedLogs.Count == 0)
                return;

            foreach (var log in unprocessedLogs)
            {
                var categoryId = await GetCategoryIdAsync(dbContext, log.ArticleId, cancellationToken);
                var points = CalculatePoints(log.ActionType);

                await UpdateRecommendationAsync(dbContext, log.UserId, categoryId, points, cancellationToken);
                MarkLogAsProcessed(log);
                await dbContext.SaveChangesAsync(cancellationToken);
            }

            logger.LogInformation("Processed {Count} user action logs.", unprocessedLogs.Count);
        }

 
        private async Task<List<UserActionLog>> GetUnprocessedLogsAsync(NewsAggregatorDbContext dbContext, CancellationToken cancellationToken) =>
            await dbContext.UserActionLogs
                .Where(log => !log.IsProcessed)
                .ToListAsync(cancellationToken);
        private async Task<int> GetCategoryIdAsync(NewsAggregatorDbContext dbContext, int articleId, CancellationToken cancellationToken) =>
            await dbContext.Articles
                .Where(a => a.Id == articleId)
                .Select(a => a.CategoryId)
                .FirstOrDefaultAsync(cancellationToken);
        private int CalculatePoints(string actionType) =>
            ActionPoints.TryGetValue(actionType.ToLower(), out var p) ? p : 0;
        private async Task UpdateRecommendationAsync(
            NewsAggregatorDbContext dbContext,
            Guid userId,
            int categoryId,
            int points,
            CancellationToken cancellationToken)
        {
            var recommendation = await dbContext.UserRecommendations
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.CategoryId == categoryId, cancellationToken);

            if (recommendation == null)
            {
                recommendation = new UserRecommendation
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    CategoryId = categoryId,
                    Points = points,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                dbContext.UserRecommendations.Add(recommendation);
            }
            else
            {
                recommendation.Points += points;
                recommendation.UpdatedAt = DateTime.UtcNow;
                dbContext.UserRecommendations.Update(recommendation);
            }
        }
        private static void MarkLogAsProcessed(UserActionLog log)
        {
            log.IsProcessed = true;
            log.UpdatedAt = DateTime.UtcNow;
        }

    }
}