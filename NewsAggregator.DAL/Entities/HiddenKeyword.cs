namespace NewsAggregator.DAL.Entities
{
    public class HiddenKeyword
    {
        public int Id { get; set; }
        public string Keyword { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;
    }

}
