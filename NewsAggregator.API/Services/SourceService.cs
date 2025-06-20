using NewsAggregator.API.Models;
using NewsAggregator.DAL.Entities;
using NewsAggregator.DAL.Repository;

namespace NewsAggregator.API.Services
{
    public interface ISourceService
    {
        Task<IEnumerable<Source>> GetAllAsync();
        Task<Source?> GetByIdAsync(int id);
        Task<Source> AddAsync(SourceRequest dto);
        Task<Source> UpdateAsync(int id, SourceUpdateRequest dto);
        Task<bool> DeleteAsync(int id);
    }

    public class SourceService(ISourceRepository repository) : ISourceService
    {
        public async Task<IEnumerable<Source>> GetAllAsync()
        {
            return await repository.GetAllSourcesAsync();
        }

        public async Task<Source?> GetByIdAsync(int id)
        {
            return await repository.GetByIdAsync(id);
        }

        public async Task<Source> AddAsync(SourceRequest request)
        {
            var newConfig = new Source
            {
                Name = request.ApiName,
                ApiUrl = request.BaseUrl,
                ApiKey = request.ApiKey,
                IsActive = true
            };
            var savedConfig = await repository.AddAsync(newConfig);
            return savedConfig;
        }

        public async Task<Source> UpdateAsync(int id, SourceUpdateRequest request)
        {
            var config = await repository.GetByIdAsync(id);

            if (request.ApiName is not null)
                config.Name = request.ApiName;

            if (request.BaseUrl is not null)
                config.ApiUrl = request.BaseUrl;

            if (request.ApiKey is not null)
                config.ApiKey = request.ApiKey;

            var updatedConfig = await repository.UpdateAsync(config);
            return updatedConfig;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await repository.DeleteAsync(id);
        }

    }
}