using Microsoft.EntityFrameworkCore;
using NewsAggregator.DAL.Context;
using NewsAggregator.DAL.Entities;

namespace NewsAggregator.DAL.Repository
{
    public interface ISourceRepository
    {
        Task<List<Source>> GetAllSourcesAsync();
        Task<Source?> GetByNameAsync(string name);
        Task<Source?> GetByIdAsync(int id);
        Task<Source> AddAsync(Source config);
        Task<Source> UpdateAsync(Source config);
        Task<Source> UpdateActiveStatusAsync(Source config, bool IsActive);
        Task<bool> DeleteAsync(int id);
    }

    public class SourceRepository(NewsAggregatorDbContext dbContext) : ISourceRepository
    {
        public async Task<List<Source>> GetAllSourcesAsync()
        {
            return await dbContext.Sources.Where(config => !config.IsDeleted).ToListAsync();
        }

        public async Task<Source?> GetByNameAsync(string name)
        {
            return await dbContext.Sources.FirstOrDefaultAsync(s => s.Name == name);
        }
     
        public async Task<Source?> GetByIdAsync(int id)
        {
            return await dbContext.Sources
                .FirstOrDefaultAsync(config => config.Id == id && !config.IsDeleted);
        }

        public async Task<Source> AddAsync(Source config)
        {
            dbContext.Sources.Add(config);
            await dbContext.SaveChangesAsync();
            return config;
        }

        public async Task<Source> UpdateAsync(Source config)
        {
            dbContext.Sources.Update(config);
            await dbContext.SaveChangesAsync();
            return config;
        }

        public async Task<Source> UpdateActiveStatusAsync(Source config, bool IsActive)
        {
            config.IsActive = IsActive;
            dbContext.Sources.Update(config);
            await dbContext.SaveChangesAsync();
            return config;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var config = await dbContext.Sources.FindAsync(id);
            if (config == null) return false;
            config.IsDeleted = true;
            config.IsActive = false;
            await dbContext.SaveChangesAsync();
            return true;
        }
    }
}