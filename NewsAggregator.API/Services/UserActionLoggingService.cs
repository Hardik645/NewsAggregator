using Microsoft.EntityFrameworkCore;
using NewsAggregator.DAL.Context;
using NewsAggregator.DAL.Entities;

namespace NewsAggregator.API.Services
{
    public interface IUserActionLoggingService
    {
        Task LogActionAsync(Guid userId, int articleId, string actionType);
    }

    public class UserActionLoggingService(NewsAggregatorDbContext dbContext) : IUserActionLoggingService
    {
        public async Task LogActionAsync(Guid userId, int articleId, string actionType)
        {
            var existingUnprocessedLog = await dbContext.UserActionLogs
                .FirstOrDefaultAsync(
                    x => x.UserId == userId
                         && x.ArticleId == articleId
                         && x.ActionType == actionType
                         && !x.IsProcessed);

            if (existingUnprocessedLog != null) return;
            
            var logEntry = new UserActionLog
            {
                UserId = userId,
                ArticleId = articleId,
                ActionType = actionType,
            };

            dbContext.UserActionLogs.Add(logEntry);
            await dbContext.SaveChangesAsync();
        }
    }
}