using NewsAggregator.DAL.Context;
using NewsAggregator.DAL.Entities;

namespace NewsAggregator.DAL.Repository
{
    public interface IArticleRepository
    {
        Task SaveArticlesAsync(IEnumerable<Article> articles);
    }
    public class ArticleRepository(NewsAggregatorDbContext dbContext) : IArticleRepository
    {
        public async Task SaveArticlesAsync(IEnumerable<Article> articles)
        {
            foreach (var article in articles)
            {
                if (!dbContext.Articles.Any(a => a.Title == article.Title))
                    dbContext.Articles.Add(article);
            }
            await dbContext.SaveChangesAsync();
        }

    }
}