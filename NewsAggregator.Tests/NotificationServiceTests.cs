using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NewsAggregator.API.Services;
using NewsAggregator.DAL.Entities;
using NewsAggregator.DAL.Repository;
using Xunit;

namespace NewsAggregator.Tests
{
    public class NotificationServiceTests
    {
        private readonly Mock<INotificationRepository> _notificationRepoMock;
        private readonly NotificationService _service;

        public NotificationServiceTests()
        {
            _notificationRepoMock = new Mock<INotificationRepository>();
            _service = new NotificationService(_notificationRepoMock.Object);
        }

        [Fact]
        public async Task GetUserNotificationsAsync_ReturnsNotifications()
        {
            var userId = Guid.NewGuid();
            var notifications = new List<Notification>
            {
                new() { Id = 1, UserId = userId, ArticleId = 1, Type = "category" }
            };
            _notificationRepoMock.Setup(r => r.GetUserNotificationsAsync(userId)).ReturnsAsync(notifications);

            var result = await _service.GetUserNotificationsAsync(userId);

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(userId, result[0].UserId);
        }

        [Fact]
        public async Task GetUserNotificationConfigAsync_ReturnsCategoriesAndKeywords()
        {
            var userId = Guid.NewGuid();
            var categories = new List<string> { "Tech", "Science" };
            var keywords = new List<string> { "AI", "ML" };
            _notificationRepoMock.Setup(r => r.GetUserEnabledCategoriesAsync(userId)).ReturnsAsync(categories);
            _notificationRepoMock.Setup(r => r.GetUserKeywordsAsync(userId)).ReturnsAsync(keywords);

            var (resultCategories, resultKeywords) = await _service.GetUserNotificationConfigAsync(userId);

            Assert.NotNull(resultCategories);
            Assert.NotNull(resultKeywords);
            Assert.Equal(2, resultCategories.Count);
            Assert.Equal(2, resultKeywords.Count);
        }

        [Fact]
        public async Task SetCategoryNotificationAsync_ReturnsTrue_WhenSuccess()
        {
            var userId = Guid.NewGuid();
            var category = "Tech";
            _notificationRepoMock.Setup(r => r.SetCategoryNotificationAsync(userId, category, true)).ReturnsAsync(true);

            var result = await _service.SetCategoryNotificationAsync(userId, category, true);

            Assert.True(result);
        }

        [Fact]
        public async Task SetCategoryNotificationAsync_ReturnsFalse_WhenFailure()
        {
            var userId = Guid.NewGuid();
            var category = "Tech";
            _notificationRepoMock.Setup(r => r.SetCategoryNotificationAsync(userId, category, false)).ReturnsAsync(false);

            var result = await _service.SetCategoryNotificationAsync(userId, category, false);

            Assert.False(result);
        }

        [Fact]
        public async Task SetKeywordNotificationsAsync_ReturnsTrue_WhenSuccess()
        {
            var userId = Guid.NewGuid();
            var keywords = new List<string> { "AI", "ML" };
            _notificationRepoMock.Setup(r => r.SetKeywordNotificationsAsync(userId, keywords)).ReturnsAsync(true);

            var result = await _service.SetKeywordNotificationsAsync(userId, keywords);

            Assert.True(result);
        }

        [Fact]
        public async Task SetKeywordNotificationsAsync_ReturnsFalse_WhenFailure()
        {
            var userId = Guid.NewGuid();
            var keywords = new List<string> { "AI", "ML" };
            _notificationRepoMock.Setup(r => r.SetKeywordNotificationsAsync(userId, keywords)).ReturnsAsync(false);

            var result = await _service.SetKeywordNotificationsAsync(userId, keywords);

            Assert.False(result);
        }
    }
}