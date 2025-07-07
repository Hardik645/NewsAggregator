namespace NewsAggregator.API.Models
{
    public class CategoryConfigDto
    {
        public string Category { get; set; } = string.Empty;
        public bool Enabled { get; set; }
    }

    public class KeywordsConfigDto
    {
        public List<string> Keywords { get; set; } = new();
    }
}