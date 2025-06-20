namespace NewsAggregator.DAL.Entities
{
    public class Source
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string ApiUrl { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime LastAccessedDate { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<Article> Articles { get; set; } = new List<Article>();
    }
}