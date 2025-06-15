namespace NewsAggregator.API.Models
{
    public class NewsApiOrgResponse
    {
        public string Status { get; set; }
        public int TotalResults { get; set; }
        public List<NewsApiOrgArticle> Articles { get; set; }
    }

    public class NewsApiOrgArticle
    {
        public NewsApiOrgSource Source { get; set; }
        public string Author { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string UrlToImage { get; set; }
        public DateTime PublishedAt { get; set; }
        public string Content { get; set; }
    }

    public class NewsApiOrgSource
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}