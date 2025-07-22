namespace NewsAggregator.DAL.Entities
{
    public class Notification
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public int ArticleId { get; set; }
        public DateTime SentAt { get; set; }
        public string Type { get; set; } = string.Empty;

        public User User { get; set; } = null!;
        public Article Article { get; set; } = null!;
    }
}