using NewsAggregator.DAL.Entities;
using NewsAggregator.DAL.Repository;

namespace NewsAggregator.API.Services
{
    public interface INotificationService
    {
        Task<List<Notification>> GetUserNotificationsAsync(Guid userId);
        Task<(List<string> Categories, List<string> Keywords)> GetUserNotificationConfigAsync(Guid userId);
        Task<bool> SetCategoryNotificationAsync(Guid userId, string category, bool enabled);
        Task<bool> SetKeywordNotificationsAsync(Guid userId, List<string> keywords);
    }

    public class NotificationService(INotificationRepository notificationRepository) : INotificationService
    {
        public async Task<List<Notification>> GetUserNotificationsAsync(Guid userId)
        {
            return await notificationRepository.GetUserNotificationsAsync(userId);
        }

        public async Task<(List<string> Categories, List<string> Keywords)> GetUserNotificationConfigAsync(Guid userId)
        {
            var categories = await notificationRepository.GetUserEnabledCategoriesAsync(userId);
            var keywords = await notificationRepository.GetUserKeywordsAsync(userId);
            return (categories, keywords);
        }

        public async Task<bool> SetCategoryNotificationAsync(Guid userId, string category, bool enabled)
        {
            return await notificationRepository.SetCategoryNotificationAsync(userId, category, enabled);
        }

        public async Task<bool> SetKeywordNotificationsAsync(Guid userId, List<string> keywords)
        {
            return await notificationRepository.SetKeywordNotificationsAsync(userId, keywords);
        }
    }
}