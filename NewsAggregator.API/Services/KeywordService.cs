using NewsAggregator.API.Models;
using NewsAggregator.DAL.Entities;
using NewsAggregator.DAL.Repository;

namespace NewsAggregator.API.Services
{
    public interface IKeywordService
    {
        Task AddKeywordsAsync(int categoryId, CreateKeywordRequest request);
        Task DeleteKeywordAsync(int keywordId);
        Task AddHiddenKeywordAsync(string keyword);
        Task RemoveHiddenKeywordAsync(string keyword);
        Task<List<HiddenKeyword>> GetAllHiddenKeywordsAsync();
        Task<int> HideArticlesWithHiddenKeywordsAsync();
    }

    public class KeywordService(
        ICategoryRepository categoryRepository,
        IKeywordRepository keyRepository,
        IArticleRepository articleRepository) : IKeywordService
    {
        public async Task AddKeywordsAsync(int categoryId, CreateKeywordRequest request)
        {
            _ = await categoryRepository.GetByIdAsync(categoryId)
                ?? throw new KeyNotFoundException($"Category with ID {categoryId} not found.");

            var keywords = request.CommaSeparatedKeywords
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(k => !string.IsNullOrWhiteSpace(k));

            await keyRepository.AddKeywordsAsync(categoryId, keywords);
        }

        public async Task DeleteKeywordAsync(int keywordId)
        {
            var result = await keyRepository.DeleteKeywordAsync(keywordId);
            if (!result)
                throw new KeyNotFoundException($"Keyword with ID {keywordId} not found.");
        }

        public async Task AddHiddenKeywordAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                throw new ArgumentException("Keyword cannot be empty.", nameof(keyword));

            var result = await keyRepository.AddHiddenKeywordAsync(keyword);
            if (!result)
                throw new ArgumentException($"Keyword already hidden.");
        }

        public async Task RemoveHiddenKeywordAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                throw new ArgumentException("Keyword cannot be empty.", nameof(keyword));

            var result = await keyRepository.RemoveHiddenKeywordAsync(keyword);
            if (!result)
                throw new KeyNotFoundException($"Keyword not found.");
        }

        public async Task<List<HiddenKeyword>> GetAllHiddenKeywordsAsync()
        {
            return await keyRepository.GetAllHiddenKeywordsAsync();
        }

        public async Task<int> HideArticlesWithHiddenKeywordsAsync()
        {
            var hiddenKeywords = await keyRepository.GetAllHiddenKeywordsAsync();
            var keywordList = hiddenKeywords
                .Where(hk => !hk.IsDeleted)
                .Select(hk => hk.Keyword)
                .ToList();

            return await articleRepository.HideArticlesWithHiddenKeywordsAsync(keywordList);
        }
    }
}