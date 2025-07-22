using System;

namespace NewsAggregator.DAL.Entities
{
    public class UserActionLog
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public int ArticleId { get; set; }
        public string ActionType { get; set; } = string.Empty; 
        public bool IsProcessed { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public User? User { get; set; }
        public Article? Article { get; set; }
    }
}