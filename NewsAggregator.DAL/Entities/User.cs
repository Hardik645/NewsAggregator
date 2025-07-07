using System;
using System.Collections.Generic;

namespace NewsAggregator.DAL.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;

        public ICollection<SavedArticle> SavedArticles { get; set; } = new List<SavedArticle>();
        public ICollection<NotificationPreference> NotificationPreferences { get; set; } = new List<NotificationPreference>();
        public ICollection<NotificationKeyword> NotificationKeywords { get; set; } = new List<NotificationKeyword>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public ICollection<ArticleFeedback> ArticleFeedbacks { get; set; } = new List<ArticleFeedback>();
    }
}

