namespace NewsAggregator.DAL.Entities
{
    public class NotificationPreference
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public int CategoryId { get; set; }
        public bool Enabled { get; set; }

        public User User { get; set; } = null!;
        public Category Category { get; set; } = null!;
    }
}