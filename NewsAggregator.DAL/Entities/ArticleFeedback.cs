namespace NewsAggregator.DAL.Entities
{
    public class ArticleFeedback
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public int ArticleId { get; set; }
        public bool IsLike { get; set; }
        public DateTime CreatedAt { get; set; }

        public User User { get; set; } = null!;
        public Article Article { get; set; } = null!;
    }
}