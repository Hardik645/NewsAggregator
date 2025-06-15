using Microsoft.EntityFrameworkCore;
using NewsAggregator.DAL.Context;
using NewsAggregator.DAL.Entities;
using System.Threading.Tasks;

namespace NewsAggregator.DAL.Repository
{
    public interface ICategoryRepository
    {
        Task<Category?> GetByIdAsync(int id);
    }

    public class CategoryRepository(NewsAggregatorDbContext _dbContext) : ICategoryRepository
    {
        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _dbContext.Categories.FindAsync(id);
        }
    }
}