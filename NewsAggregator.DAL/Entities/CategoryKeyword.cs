namespace NewsAggregator.DAL.Entities
{
    public class CategoryKeyword
    {
        public int Id { get; set; }
        public string Keyword { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public Category Category { get; set; } = default!;
    }
}