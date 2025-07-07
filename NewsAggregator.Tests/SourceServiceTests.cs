using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NewsAggregator.API.Models;
using NewsAggregator.API.Services;
using NewsAggregator.DAL.Entities;
using NewsAggregator.DAL.Repository;
using Xunit;

namespace NewsAggregator.Tests
{
    public class SourceServiceTests
    {
        private readonly Mock<ISourceRepository> _sourceRepoMock;
        private readonly SourceService _service;

        public SourceServiceTests()
        {
            _sourceRepoMock = new Mock<ISourceRepository>();
            _service = new SourceService(_sourceRepoMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsSources()
        {
            var sources = new List<Source>
            {
                new() { Id = 1, Name = "NewsApiOrg", ApiUrl = "http://api.org", ApiKey = "key1", IsActive = true },
                new() { Id = 2, Name = "TheNewsApi", ApiUrl = "http://thenewsapi.com", ApiKey = "key2", IsActive = true }
            };
            _sourceRepoMock.Setup(r => r.GetAllSourcesAsync()).ReturnsAsync(sources);

            var result = await _service.GetAllAsync();

            Assert.NotNull(result);
            Assert.Equal(2, ((List<Source>)result).Count);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsSource_WhenExists()
        {
            var source = new Source { Id = 1, Name = "NewsApiOrg", ApiUrl = "http://api.org", ApiKey = "key1", IsActive = true };
            _sourceRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(source);

            var result = await _service.GetByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
        {
            _sourceRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Source?)null);

            var result = await _service.GetByIdAsync(99);

            Assert.Null(result);
        }

        [Fact]
        public async Task AddAsync_AddsSource()
        {
            var request = new SourceRequest { ApiName = "NewsApiOrg", BaseUrl = "http://api.org", ApiKey = "key1" };
            var source = new Source { Id = 1, Name = "NewsApiOrg", ApiUrl = "http://api.org", ApiKey = "key1", IsActive = true };
            _sourceRepoMock.Setup(r => r.AddAsync(It.IsAny<Source>())).ReturnsAsync(source);

            var result = await _service.AddAsync(request);

            Assert.NotNull(result);
            Assert.Equal("NewsApiOrg", result.Name);
            Assert.Equal("http://api.org", result.ApiUrl);
            Assert.Equal("key1", result.ApiKey);
            Assert.True(result.IsActive);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesSource()
        {
            var updateRequest = new SourceUpdateRequest { ApiName = "UpdatedApi", BaseUrl = "http://updated.org", ApiKey = "updatedkey" };
            var existing = new Source { Id = 1, Name = "OldApi", ApiUrl = "http://old.org", ApiKey = "oldkey", IsActive = true };
            var updated = new Source { Id = 1, Name = "UpdatedApi", ApiUrl = "http://updated.org", ApiKey = "updatedkey", IsActive = true };

            _sourceRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
            _sourceRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Source>())).ReturnsAsync(updated);

            var result = await _service.UpdateAsync(1, updateRequest);

            Assert.NotNull(result);
            Assert.Equal("UpdatedApi", result.Name);
            Assert.Equal("http://updated.org", result.ApiUrl);
            Assert.Equal("updatedkey", result.ApiKey);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsTrue_WhenDeleted()
        {
            _sourceRepoMock.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

            var result = await _service.DeleteAsync(1);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsFalse_WhenNotDeleted()
        {
            _sourceRepoMock.Setup(r => r.DeleteAsync(99)).ReturnsAsync(false);

            var result = await _service.DeleteAsync(99);

            Assert.False(result);
        }
    }
}