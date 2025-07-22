using Moq;
using NewsAggregator.API.Models;
using NewsAggregator.API.Services;
using NewsAggregator.DAL.Entities;
using NewsAggregator.DAL.Repository;
using Xunit;

namespace NewsAggregator.Tests
{
    public class KeywordServiceTests
    {
        private readonly Mock<ICategoryRepository> _categoryRepoMock;
        private readonly Mock<IKeywordRepository> _keywordRepoMock;
        private readonly Mock<IArticleRepository> _articleRepoMock;
        private readonly KeywordService _service;

        public KeywordServiceTests()
        {
            _categoryRepoMock = new Mock<ICategoryRepository>();
            _keywordRepoMock = new Mock<IKeywordRepository>();
            _articleRepoMock = new Mock<IArticleRepository>();
            _service = new KeywordService(_categoryRepoMock.Object, _keywordRepoMock.Object, _articleRepoMock.Object);
        }

        [Fact]
        public async Task DeleteKeywordAsync_CallsRepository()
        {
            var keywordId = 1;
            _keywordRepoMock.Setup(r => r.DeleteKeywordAsync(keywordId)).ReturnsAsync(true).Verifiable();

            await _service.DeleteKeywordAsync(keywordId);

            _keywordRepoMock.Verify(r => r.DeleteKeywordAsync(keywordId), Times.Once);
        }

        [Fact]
        public async Task AddHiddenKeywordAsync_CallsRepository()
        {
            var keyword = "spam";
            _keywordRepoMock.Setup(r => r.AddHiddenKeywordAsync(keyword)).ReturnsAsync(true).Verifiable();

            await _service.AddHiddenKeywordAsync(keyword);

            _keywordRepoMock.Verify(r => r.AddHiddenKeywordAsync(keyword), Times.Once);
        }

        [Fact]
        public async Task RemoveHiddenKeywordAsync_CallsRepository()
        {
            var keyword = "spam";
            _keywordRepoMock.Setup(r => r.RemoveHiddenKeywordAsync(keyword)).ReturnsAsync(true).Verifiable();

            await _service.RemoveHiddenKeywordAsync(keyword);

            _keywordRepoMock.Verify(r => r.RemoveHiddenKeywordAsync(keyword), Times.Once);
        }

        [Fact]
        public async Task GetAllHiddenKeywordsAsync_ReturnsList()
        {
            var hiddenKeywords = new List<HiddenKeyword>
            {
                new() { Id = 1, Keyword = "spam" },
                new() { Id = 2, Keyword = "ads" }
            };
            _keywordRepoMock.Setup(r => r.GetAllHiddenKeywordsAsync()).ReturnsAsync(hiddenKeywords);

            var result = await _service.GetAllHiddenKeywordsAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }
    }
}           