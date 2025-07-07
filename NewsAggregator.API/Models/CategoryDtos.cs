namespace NewsAggregator.API.Models
{
    public class CreateKeywordRequest
    {
        public string CommaSeparatedKeywords { get; set; }
    }
    public class CategoryResponse
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public List<string> Keywords { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
