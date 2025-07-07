namespace NewsAggregator.DAL.Entities
{
    public class NotificationKeyword
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public string Keyword { get; set; } = string.Empty;

        public User User { get; set; } = null!;
    }
}