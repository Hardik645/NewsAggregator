using Microsoft.EntityFrameworkCore;
using NewsAggregator.DAL.Context;
using NewsAggregator.DAL.Entities;
using System.Threading.Tasks;

namespace NewsAggregator.DAL.Repository
{
    public interface IKeywordRepository
    {
        Task<List<CategoryKeyword>> GetKeywordsByCategoryIdAsync(int categoryId);
        Task AddKeywordsAsync(int categoryId, IEnumerable<string> keywords);
        Task<bool> DeleteKeywordAsync(int keywordId);
        Task<bool> AddHiddenKeywordAsync(string keyword);
        Task<bool> RemoveHiddenKeywordAsync(string keyword);
        Task<List<HiddenKeyword>> GetAllHiddenKeywordsAsync();
    }

    public class KeywordRepository(NewsAggregatorDbContext _dbContext) : IKeywordRepository
    {
        public async Task AddKeywordsAsync(int categoryId, IEnumerable<string> keywords)
        {
            var keywordEntities = keywords.Select(k => new CategoryKeyword
            {
                CategoryId = categoryId,
                Keyword = k.Trim(),
                IsDeleted = false
            });

            await _dbContext.CategoryKeywords.AddRangeAsync(keywordEntities);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<bool> DeleteKeywordAsync(int keywordId)
        {
            var keyword = await _dbContext.CategoryKeywords.FirstOrDefaultAsync(k => k.Id == keywordId && !k.IsDeleted);
            if (keyword == null || keyword.IsDeleted) return false;

            keyword.IsDeleted = true;
            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<List<CategoryKeyword>> GetKeywordsByCategoryIdAsync(int categoryId)
        {
            return await _dbContext.CategoryKeywords
                .Where(k => k.CategoryId == categoryId && !k.IsDeleted)
                .ToListAsync();
        }
        public async Task<bool> AddHiddenKeywordAsync(string keyword)
        {
            var trimmedKeyword = keyword.Trim();
            var exists = await _dbContext.HiddenKeywords
                .AnyAsync(hk => hk.Keyword.ToLower() == trimmedKeyword.ToLower());
            if (exists)
            {
                return false;
            }

            var hiddenKeyword = new HiddenKeyword
            {
                Keyword = trimmedKeyword
            };
            _dbContext.HiddenKeywords.Add(hiddenKeyword);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> RemoveHiddenKeywordAsync(string keyword)
        {
            var hiddenKeyword = await _dbContext.HiddenKeywords
                .FirstOrDefaultAsync(hk => hk.Keyword == keyword.Trim());
            if (hiddenKeyword == null) return false;

            _dbContext.HiddenKeywords.Remove(hiddenKeyword);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<List<HiddenKeyword>> GetAllHiddenKeywordsAsync()
        {
            return await _dbContext.HiddenKeywords
                .Where(hk => !hk.IsDeleted)
                .ToListAsync();
        }
    }
}