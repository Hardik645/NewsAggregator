using NewsAggregator.API.Models;
using NewsAggregator.DAL.Entities;
using NewsAggregator.DAL.Repository;

namespace NewsAggregator.API.Services
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAllCategoryAsync();
        Task<CategoryResponse> CreateCategoryAsync(string name);
        Task<CategoryResponse> GetCategoryWithKeywordsAsync(int categoryId);
        Task AddKeywordsAsync(int categoryId, CreateKeywordRequest request);
        Task DeleteCategoryAsync(int categoryId);
        Task DeleteKeywordAsync(int keywordId);
    }

    public class CategoryService(ICategoryRepository repository) : ICategoryService
    {
        public async Task<List<Category>> GetAllCategoryAsync()
            => await repository.GetAllAsync();
        public async Task<CategoryResponse> CreateCategoryAsync(string name)
        {
            var category = new Category
            {
                Name = name,
                IsDeleted = false
            };

            var saved = await repository.AddCategoryAsync(category);

            return new CategoryResponse
            {
                CategoryId = saved.Id,
                CategoryName = saved.Name,
                CreatedAt = DateTime.UtcNow,
                Keywords = []
            };
        }

        public async Task<CategoryResponse> GetCategoryWithKeywordsAsync(int categoryId)
        {
            var category = await repository.GetByIdAsync(categoryId)
                ?? throw new KeyNotFoundException($"Category with ID {categoryId} not found.");

            var keywords = await repository.GetKeywordsByCategoryIdAsync(categoryId)
                ?? throw new KeyNotFoundException($"No keywords found for category ID {categoryId}.");

            return new CategoryResponse
            {
                CategoryId = category.Id,
                CategoryName = category.Name,
                CreatedAt = DateTime.UtcNow,
                Keywords = keywords.Select(k => k.Keyword).ToList()
            };
        }

        public async Task AddKeywordsAsync(int categoryId, CreateKeywordRequest request)
        {
            var category = await repository.GetByIdAsync(categoryId)
                ?? throw new KeyNotFoundException($"Category with ID {categoryId} not found.");

            var keywords = request.CommaSeparatedKeywords
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(k => !string.IsNullOrWhiteSpace(k));

            await repository.AddKeywordsAsync(categoryId, keywords);
        }

        public async Task DeleteCategoryAsync(int categoryId)
        {
            var result = await repository.DeleteCategoryAsync(categoryId);
            if (!result)
                throw new KeyNotFoundException($"Category with ID {categoryId} not found.");
        }

        public async Task DeleteKeywordAsync(int keywordId)
        {
            var result = await repository.DeleteKeywordAsync(keywordId);
            if (!result)
                throw new KeyNotFoundException($"Keyword with ID {keywordId} not found.");
        }
    }
}