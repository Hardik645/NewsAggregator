using Microsoft.EntityFrameworkCore;
using NewsAggregator.DAL.Context;
using NewsAggregator.DAL.Entities;
using System.Threading.Tasks;

namespace NewsAggregator.DAL.Repository
{
    public interface ISourceRepository
    {
        Task<List<Source>> GetAllSourcesAsync();
        Task<Source?> GetByNameAsync(string name);
    }

    public class SourceRepository(NewsAggregatorDbContext dbContext) : ISourceRepository
    {
        public async Task<List<Source>> GetAllSourcesAsync()
        {
            return await dbContext.Sources.ToListAsync();
        }

        public async Task<Source?> GetByNameAsync(string name)
        {
            return await dbContext.Sources.FirstOrDefaultAsync(s => s.Name == name);
        }
    }
}