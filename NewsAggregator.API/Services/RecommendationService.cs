using Microsoft.EntityFrameworkCore;
using NewsAggregator.DAL.Context;
using NewsAggregator.DAL.Entities;

namespace NewsAggregator.API.Services;

public interface IRecommendationService
{
    Task<List<Article>> SortArticlesByUserScoreAsync(Guid userId, List<Article> articles);
}

public class RecommendationService(NewsAggregatorDbContext dbContext) : IRecommendationService
{
    public async Task<List<Article>> SortArticlesByUserScoreAsync(Guid userId, List<Article> articles)
    {
        var userRecs = await dbContext.UserRecommendations
            .Where(ur => ur.UserId == userId)
            .ToListAsync();

        return articles
            .OrderByDescending(a =>
            {
                var rec = userRecs.FirstOrDefault(ur => ur.CategoryId == a.CategoryId);
                return rec?.Points ?? 0;
            })
            .ToList();
    }
}