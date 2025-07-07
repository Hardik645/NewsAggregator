using Microsoft.EntityFrameworkCore;
using NewsAggregator.DAL.Context;
using NewsAggregator.DAL.Entities;
using System.Threading.Tasks;

namespace NewsAggregator.DAL.Repository
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllAsync();
        Task<Category?> GetByNameAsync(string name);
        Task<Category?> GetByIdAsync(int id);
        Task<Category> AddCategoryAsync(Category category);
        Task<bool> DeleteCategoryAsync(int id);
        Task SetCategoryVisibilityAsync(int categoryId, bool isHidden);
    }

    public class CategoryRepository(NewsAggregatorDbContext _dbContext) : ICategoryRepository
    {
        public async Task<List<Category>> GetAllAsync()
        {
            return await _dbContext.Categories
                .AsNoTracking()
                .Include(c => c.CategoryKeywords)
                .Where(c => !c.IsDeleted)
                .Select(c => new Category
                {
                    Id = c.Id,
                    Name = c.Name,
                    IsDeleted = c.IsDeleted,
                    IsHidden = c.IsHidden,
                    CategoryKeywords = c.CategoryKeywords
                        .Where(ck => !ck.IsDeleted)
                        .Select(ck => new CategoryKeyword
                        {
                            Id = ck.Id,
                            Keyword = ck.Keyword,
                            CategoryId = ck.CategoryId,
                            IsDeleted = ck.IsDeleted
                        }).ToList()
                })
                .ToListAsync();
        }
        public async Task<Category?> GetByNameAsync(string name)
        {
            return await _dbContext.Categories
                .FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower() && !c.IsDeleted && !c.IsHidden);
        }
        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }
        public async Task<Category> AddCategoryAsync(Category category)
        {
            _dbContext.Categories.Add(category);
            await _dbContext.SaveChangesAsync();
            return category;
        }
        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _dbContext.Categories.FindAsync(id);
            if (category == null || category.IsDeleted) return false;

            category.IsDeleted = true;
            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task SetCategoryVisibilityAsync(int categoryId, bool isHidden)
        {
            var category = await _dbContext.Categories.FindAsync(categoryId);
            if (category == null || category.IsDeleted)
                throw new KeyNotFoundException($"Category with ID {categoryId} not found.");
            category.IsHidden = isHidden;
            await _dbContext.SaveChangesAsync();
        }
    }
}