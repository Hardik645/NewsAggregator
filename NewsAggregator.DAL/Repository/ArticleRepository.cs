using Microsoft.EntityFrameworkCore;
using NewsAggregator.DAL.Context;
using NewsAggregator.DAL.Entities;

namespace NewsAggregator.DAL.Repository
{
    public interface IArticleRepository
    {
        Task SaveArticlesAsync(IEnumerable<Article> articles);
        Task<int> CategorizeUnknownArticlesByKeywordsAsync();
    }
    public class ArticleRepository(NewsAggregatorDbContext dbContext) : IArticleRepository
    {
        public async Task<int> CategorizeUnknownArticlesByKeywordsAsync()
        {
            var unknownCategory = await dbContext.Categories
                .FirstOrDefaultAsync(c => c.Name.ToLower() == "unknown"); 
            if (unknownCategory == null) return 0;

            var keywords = await dbContext.CategoryKeywords.Include(ck => ck.Category).ToListAsync();
            var articles = await dbContext.Articles
                .Where(a => a.CategoryId == unknownCategory.Id)
                .ToListAsync();

            int updated = 0;
            foreach (var article in articles)
            {
                var match = keywords.FirstOrDefault(ck =>
                    article.Title.Contains(ck.Keyword, StringComparison.OrdinalIgnoreCase) ||
                    article.Content.Contains(ck.Keyword, StringComparison.OrdinalIgnoreCase));

                if (match != null)
                {
                    article.CategoryId = match.CategoryId;
                    updated++;
                }
            }

            if (updated > 0)
                await dbContext.SaveChangesAsync();

            return updated;
        }
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