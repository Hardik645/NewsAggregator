using System.Collections.Generic;

namespace NewsAggregator.DAL.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;

        public ICollection<Article> Articles { get; set; } = new List<Article>();
        public ICollection<NotificationPreference> NotificationPreferences { get; set; } = new List<NotificationPreference>();
        public ICollection<CategoryKeyword> CategoryKeywords { get; set; } = new List<CategoryKeyword>();
    }
}   