using Moq;
using NewsAggregator.API.Services;
using NewsAggregator.DAL.Entities;
using NewsAggregator.DAL.Repository;
using Xunit;

namespace NewsAggregator.Tests
{
    public class CategoryServiceTests
    {
        private readonly Mock<ICategoryRepository> _categoryRepoMock;
        private readonly Mock<IKeywordRepository> _keywordRepoMock;
        private readonly CategoryService _service;

        public CategoryServiceTests()
        {
            _categoryRepoMock = new Mock<ICategoryRepository>();
            _keywordRepoMock = new Mock<IKeywordRepository>();
            _service = new CategoryService(_categoryRepoMock.Object, _keywordRepoMock.Object);
        }


        [Fact]
        public async Task GetAllCategoryAsync_ReturnsListOfCategories()
        {
            var categories = new List<Category>
            {
                new() { Id = 1, Name = "Tech" },
                new() { Id = 2, Name = "Science" }
            };
            _categoryRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(categories);

            var result = await _service.GetAllCategoryAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task CreateCategoryAsync_ReturnsCategoryResponse()
        {
            var name = "Health";
            var category = new Category { Id = 3, Name = name };
            _categoryRepoMock.Setup(r => r.GetByNameAsync(name)).ReturnsAsync((Category?)null);
            _categoryRepoMock.Setup(r => r.AddCategoryAsync(It.IsAny<Category>())).ReturnsAsync(category);

            var result = await _service.CreateCategoryAsync(name);

            Assert.NotNull(result);
            Assert.Equal(3, result.CategoryId);
            Assert.Equal(name, result.CategoryName);
        }

        [Fact]
        public async Task GetCategoryWithKeywordsAsync_ReturnsCategoryResponse()
        {
            var categoryId = 1;
            var category = new Category { Id = categoryId, Name = "Tech" };
            var keywords = new List<CategoryKeyword>
            {
                new() { Id = 1, Keyword = "AI", CategoryId = categoryId },
                new() { Id = 2, Keyword = "ML", CategoryId = categoryId }
            };
            _categoryRepoMock.Setup(r => r.GetByIdAsync(categoryId)).ReturnsAsync(category);
            _keywordRepoMock.Setup(r => r.GetKeywordsByCategoryIdAsync(categoryId)).ReturnsAsync(keywords);

            var result = await _service.GetCategoryWithKeywordsAsync(categoryId);

            Assert.NotNull(result);
            Assert.Equal(categoryId, result.CategoryId);
            Assert.Equal("Tech", result.CategoryName);
            Assert.Equal(2, result.Keywords.Count);
        }

        [Fact]
        public async Task DeleteCategoryAsync_DeletesCategory()
        {
            var categoryId = 1;
            _categoryRepoMock.Setup(r => r.DeleteCategoryAsync(categoryId)).ReturnsAsync(true);

            await _service.DeleteCategoryAsync(categoryId);

            _categoryRepoMock.Verify(r => r.DeleteCategoryAsync(categoryId), Times.Once);
        }

        [Fact]
        public async Task SetCategoryVisibility_SetsVisibility()
        {
            var categoryId = 1;
            var isHidden = true;
            _categoryRepoMock.Setup(r => r.SetCategoryVisibilityAsync(categoryId, isHidden)).Returns(Task.CompletedTask);

            await _service.SetCategoryVisibility(categoryId, isHidden);

            _categoryRepoMock.Verify(r => r.SetCategoryVisibilityAsync(categoryId, isHidden), Times.Once);
        }
    }
}