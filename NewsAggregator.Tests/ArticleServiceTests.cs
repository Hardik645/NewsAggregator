using Moq;
using NewsAggregator.API.Services;
using NewsAggregator.DAL.Entities;
using NewsAggregator.DAL.Repository;

namespace NewsAggregator.Tests
{
    public class ArticleServiceTests
    {
        private readonly Mock<IArticleRepository> _articleRepoMock;
        private readonly ArticleService _service;

        public ArticleServiceTests()
        {
            _articleRepoMock = new Mock<IArticleRepository>();
            _service = new ArticleService(_articleRepoMock.Object);
        }

        [Fact]
        public async Task GetArticleByIdAsync_ReturnsArticle_WhenArticleExists()
        {
            var articleId = 1;
            var expectedArticle = new Article { Id = articleId, Title = "Test", Content = "Content" };
            _articleRepoMock.Setup(r => r.GetArticleByIdAsync(articleId)).ReturnsAsync(expectedArticle);

            var result = await _service.GetArticleByIdAsync(articleId);

            Assert.NotNull(result);
            Assert.Equal(articleId, result.Id);
        }

        [Fact]
        public async Task GetArticleByIdAsync_ReturnsNull_WhenArticleDoesNotExist()
        {
            var articleId = 2;
            _articleRepoMock.Setup(r => r.GetArticleByIdAsync(articleId)).ReturnsAsync((Article?)null);

            var result = await _service.GetArticleByIdAsync(articleId);

            Assert.Null(result);
        }

        [Fact]
        public async Task SaveArticleAsync_ReturnsTrue_WhenArticleIsSaved()
        {
            var articleId = 1;
            var userId = Guid.NewGuid();
            _articleRepoMock.Setup(r => r.SaveArticleForUserAsync(articleId, userId)).ReturnsAsync(true);

            var result = await _service.SaveArticleAsync(articleId, userId);

            Assert.True(result);
        }

        [Fact]
        public async Task SaveArticleAsync_ReturnsFalse_WhenArticleAlreadySaved()
        {
            var articleId = 1;
            var userId = Guid.NewGuid();
            _articleRepoMock.Setup(r => r.SaveArticleForUserAsync(articleId, userId)).ReturnsAsync(false);

            var result = await _service.SaveArticleAsync(articleId, userId);

            Assert.False(result);
        }

        [Fact]
        public async Task GetSavedArticlesAsync_ReturnsListOfArticles()
        {
            var userId = Guid.NewGuid();
            var articles = new List<Article>
            {
                new() { Id = 1, Title = "A" },
                new() { Id = 2, Title = "B" }
            };
            _articleRepoMock.Setup(r => r.GetSavedArticlesForUserAsync(userId)).ReturnsAsync(articles);

            var result = await _service.GetSavedArticlesAsync(userId);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task DeleteSavedArticleAsync_ReturnsTrue_WhenDeleted()
        {
            var articleId = 1;
            var userId = Guid.NewGuid();
            _articleRepoMock.Setup(r => r.DeleteSavedArticleForUserAsync(articleId, userId)).ReturnsAsync(true);

            var result = await _service.DeleteSavedArticleAsync(articleId, userId);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteSavedArticleAsync_ReturnsFalse_WhenNotDeleted()
        {
            var articleId = 1;
            var userId = Guid.NewGuid();
            _articleRepoMock.Setup(r => r.DeleteSavedArticleForUserAsync(articleId, userId)).ReturnsAsync(false);

            var result = await _service.DeleteSavedArticleAsync(articleId, userId);

            Assert.False(result);
        }

        [Fact]
        public async Task SetArticleFeedbackAsync_ReturnsTrue_WhenFeedbackSet()
        {
            var articleId = 1;
            var userId = Guid.NewGuid();
            bool? isLike = true;
            bool? isReported = false;
            _articleRepoMock.Setup(r => r.SetArticleFeedbackAsync(articleId, userId, isLike, isReported)).ReturnsAsync(true);

            var result = await _service.SetArticleFeedbackAsync(articleId, userId, isLike, isReported);

            Assert.True(result);
        }

        [Fact]
        public async Task SetArticleFeedbackAsync_ReturnsFalse_WhenFeedbackNotSet()
        {
            var articleId = 1;
            var userId = Guid.NewGuid();
            bool? isLike = false;
            bool? isReported = true;
            _articleRepoMock.Setup(r => r.SetArticleFeedbackAsync(articleId, userId, isLike, isReported)).ReturnsAsync(false);

            var result = await _service.SetArticleFeedbackAsync(articleId, userId, isLike, isReported);

            Assert.False(result);
        }

        [Fact]
        public async Task GetAllReportedNotHiddenArticlesAsync_ReturnsArticles()
        {
            var articles = new List<Article>
            {
                new() { Id = 1, Title = "Reported" }
            };
            _articleRepoMock.Setup(r => r.GetAllReportedNotHiddenArticlesAsync()).ReturnsAsync(articles);

            var result = await _service.GetAllReportedNotHiddenArticlesAsync();

            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task HideArticlesAsync_CallsRepository()
        {
            var id = 1;
            _articleRepoMock.Setup(r => r.HideArticlesAsync(id)).Returns(Task.CompletedTask).Verifiable();

            await _service.HideArticlesAsync(id);

            _articleRepoMock.Verify(r => r.HideArticlesAsync(id), Times.Once);
        }
    }
}