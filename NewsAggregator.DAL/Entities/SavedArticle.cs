namespace NewsAggregator.DAL.Entities
{
    public class SavedArticle
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public int ArticleId { get; set; }
        public DateTime SavedAt { get; set; }

        public User User { get; set; } = null!;
        public Article Article { get; set; } = null!;
    }
}