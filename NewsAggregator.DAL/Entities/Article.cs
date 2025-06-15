using System;
using System.Collections.Generic;

namespace NewsAggregator.DAL.Entities
{
    public class Article
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public string Url { get; set; } = default!;
        public string Content { get; set; } = default!;
        public DateTime PublishedAt { get; set; }
        public int SourceId { get; set; }
        public Source Source { get; set; } = default!;
        public int CategoryId { get; set; }
        public Category Category { get; set; } = default!;

        public ICollection<SavedArticle> SavedArticles { get; set; } = new List<SavedArticle>();
        public ICollection<ArticleFeedback> ArticleFeedbacks { get; set; } = new List<ArticleFeedback>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}