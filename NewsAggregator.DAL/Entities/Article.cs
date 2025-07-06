using System;
using System.Collections.Generic;

namespace NewsAggregator.DAL.Entities
{
    public class Article
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime PublishedAt { get; set; }
        public int SourceId { get; set; }
        public Source Source { get; set; } = null!;
        public int CategoryId { get; set; }
        public int Likes { get; set; } = 0;
        public int Dislikes { get; set; } = 0;
        public int ReportCount { get; set; } = 0;
        public bool IsHidden { get; set; } = false;
        public Category Category { get; set; } = null!;
        public ICollection<SavedArticle> SavedArticles { get; set; } = new List<SavedArticle>();
        public ICollection<ArticleFeedback> ArticleFeedbacks { get; set; } = new List<ArticleFeedback>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    }
}