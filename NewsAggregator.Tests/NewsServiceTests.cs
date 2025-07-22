using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NewsAggregator.API.Services;
using NewsAggregator.DAL.Entities;
using NewsAggregator.DAL.Repository;
using Xunit;

namespace NewsAggregator.Tests
{
    public class NewsServiceTests
    {
        private readonly Mock<IArticleRepository> _articleRepoMock;
        private readonly NewsService _service;

        public NewsServiceTests()
        {
            _articleRepoMock = new Mock<IArticleRepository>();
            _service = new NewsService(_articleRepoMock.Object);
        }

        [Fact]
        public async Task GetTodaysNewsAsync_ReturnsArticles_WhenCategoryIsNull()
        {
            var today = DateTime.UtcNow.Date;
            var articles = new List<Article> { new() { Id = 1, Title = "A" } };
            _articleRepoMock
                .Setup(r => r.GetArticlesByDateRangeAsync(
                    DateOnly.FromDateTime(today),
                    DateOnly.FromDateTime(today.AddDays(1))))
                .ReturnsAsync(articles);

            var result = await _service.GetTodaysNewsAsync(null);

            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetTodaysNewsAsync_ReturnsArticles_WhenCategoryIsAll()
        {
            var today = DateTime.UtcNow.Date;
            var articles = new List<Article> { new() { Id = 1, Title = "A" } };
            _articleRepoMock
                .Setup(r => r.GetArticlesByDateRangeAsync(
                    DateOnly.FromDateTime(today),
                    DateOnly.FromDateTime(today.AddDays(1))))
                .ReturnsAsync(articles);

            var result = await _service.GetTodaysNewsAsync("all");

            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetTodaysNewsAsync_ReturnsArticles_ByCategory()
        {
            var today = DateTime.UtcNow.Date;
            var articles = new List<Article> { new() { Id = 2, Title = "B" } };
            _articleRepoMock
                .Setup(r => r.GetArticlesByCategoryAndDateAsync("Tech", today))
                .ReturnsAsync(articles);

            var result = await _service.GetTodaysNewsAsync("Tech");

            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetTodaysNewsAsync_ThrowsApplicationException_OnError()
        {
            _articleRepoMock
                .Setup(r => r.GetArticlesByDateRangeAsync(It.IsAny<DateOnly>(), It.IsAny<DateOnly>()))
                .ThrowsAsync(new Exception("db error"));

            await Assert.ThrowsAsync<ApplicationException>(() => _service.GetTodaysNewsAsync(null));
        }

        [Fact]
        public async Task GetNewsByDateRangeAsync_ReturnsArticles_WhenCategoryIsNull()
        {
            var start = new DateOnly(2024, 1, 1);
            var end = new DateOnly(2024, 1, 2);
            var articles = new List<Article> { new() { Id = 1, Title = "A" } };
            _articleRepoMock
                .Setup(r => r.GetArticlesByDateRangeAsync(start, end))
                .ReturnsAsync(articles);

            var result = await _service.GetNewsByDateRangeAsync(start, end, null);

            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetNewsByDateRangeAsync_ReturnsArticles_ByCategory()
        {
            var start = new DateOnly(2024, 1, 1);
            var end = new DateOnly(2024, 1, 2);
            var articles = new List<Article> { new() { Id = 2, Title = "B" } };
            _articleRepoMock
                .Setup(r => r.GetArticlesByCategoryAndDateRangeAsync("Tech", start, end))
                .ReturnsAsync(articles);

            var result = await _service.GetNewsByDateRangeAsync(start, end, "Tech");

            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetNewsByDateRangeAsync_ThrowsApplicationException_OnError()
        {
            var start = new DateOnly(2024, 1, 1);
            var end = new DateOnly(2024, 1, 2);
            _articleRepoMock
                .Setup(r => r.GetArticlesByDateRangeAsync(start, end))
                .ThrowsAsync(new Exception("db error"));

            await Assert.ThrowsAsync<ApplicationException>(() => _service.GetNewsByDateRangeAsync(start, end, null));
        }

        [Fact]
        public async Task GetTodaysNewsByCategoryAsync_ReturnsArticles()
        {
            var today = DateTime.UtcNow.Date;
            var articles = new List<Article> { new() { Id = 3, Title = "C" } };
            _articleRepoMock
                .Setup(r => r.GetArticlesByCategoryAndDateAsync("Tech", today))
                .ReturnsAsync(articles);

            var result = await _service.GetTodaysNewsByCategoryAsync("Tech");

            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetTodaysNewsByCategoryAsync_ThrowsApplicationException_OnError()
        {
            var today = DateTime.UtcNow.Date;
            _articleRepoMock
                .Setup(r => r.GetArticlesByCategoryAndDateAsync("Tech", today))
                .ThrowsAsync(new Exception("db error"));

            await Assert.ThrowsAsync<ApplicationException>(() => _service.GetTodaysNewsByCategoryAsync("Tech"));
        }

        [Fact]
        public async Task SearchNewsAsync_ReturnsArticles()
        {
            var articles = new List<Article> { new() { Id = 4, Title = "D" } };
            _articleRepoMock
                .Setup(r => r.SearchArticlesAsync("query", null, null, null))
                .ReturnsAsync(articles);

            var result = await _service.SearchNewsAsync("query", null, null, null);

            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task SearchNewsAsync_ThrowsApplicationException_OnError()
        {
            _articleRepoMock
                .Setup(r => r.SearchArticlesAsync(It.IsAny<string>(), It.IsAny<DateOnly?>(), It.IsAny<DateOnly?>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("db error"));

            await Assert.ThrowsAsync<ApplicationException>(() => _service.SearchNewsAsync("query", null, null, null));
        }
    }
}