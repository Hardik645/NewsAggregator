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
        Task DeleteCategoryAsync(int categoryId);
        Task SetCategoryVisibility(int categoryId, bool isHidden);
    }

    public class CategoryService(ICategoryRepository categoryRepository, IKeywordRepository keywordRepository) : ICategoryService
    {
        public async Task<List<Category>> GetAllCategoryAsync()
            => await categoryRepository.GetAllAsync();
        public async Task<CategoryResponse> CreateCategoryAsync(string name)
        {
            var category = new Category
            {
                Name = name,
                IsDeleted = false
            };

            var saved = await categoryRepository.AddCategoryAsync(category);

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
            var category = await categoryRepository.GetByIdAsync(categoryId)
                ?? throw new KeyNotFoundException($"Category with ID {categoryId} not found.");

            var keywords = await keywordRepository.GetKeywordsByCategoryIdAsync(categoryId)
                ?? throw new KeyNotFoundException($"No keywords found for category ID {categoryId}.");

            return new CategoryResponse
            {
                CategoryId = category.Id,
                CategoryName = category.Name,
                CreatedAt = DateTime.UtcNow,
                Keywords = keywords.Select(k => k.Keyword).ToList()
            };
        }
        public async Task DeleteCategoryAsync(int categoryId)
        {
            var result = await categoryRepository.DeleteCategoryAsync(categoryId);
            if (!result)
                throw new KeyNotFoundException($"Category with ID {categoryId} not found.");
        }
        public async Task SetCategoryVisibility(int categoryId, bool isHidden)
        {
            await categoryRepository.SetCategoryVisibilityAsync(categoryId, isHidden);
        }
    }
}