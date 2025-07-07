using System;

namespace NewsAggregator.DAL.Entities
{
    public class UserRecommendation
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public int CategoryId { get; set; }
        public int Points { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public User? User { get; set; } 
        public Category? Category { get; set; }
    }
}