using CraqForge.Core.Pagination;

namespace CraqForge.Core.Tests
{
    public class PaginatedTests
    {
        [Fact]
        public void Constructor_ShouldInitializePropertiesCorrectly()
        {
            // Arrange
            var items = new List<string> { "Item1", "Item2" };
            int totalCount = 10;
            int page = 2;
            int pageSize = 5;

            // Act
            var paginated = new Paginated<string>(items, totalCount, page, pageSize);

            // Assert
            Assert.Equal(items, paginated.Items);
            Assert.Equal(totalCount, paginated.TotalCount);
            Assert.Equal(page, paginated.Page);
            Assert.Equal(pageSize, paginated.PageSize);
            Assert.Equal(2, paginated.TotalPages);
            Assert.True(paginated.HasPrevious);
            Assert.False(paginated.HasNext);
        }

        [Fact]
        public void TotalPages_ShouldCalculateCorrectly()
        {
            // Act
            var paginated = new Paginated<int>([], totalCount: 23, page: 1, pageSize: 10);

            // Assert
            Assert.Equal(3, paginated.TotalPages);
        }

        [Theory]
        [InlineData(1, 10, false)]
        [InlineData(2, 10, true)]
        public void HasPrevious_ShouldReturnExpectedValue(int page, int totalCount, bool expected)
        {
            var paginated = new Paginated<string>([], totalCount, page, 5);
            Assert.Equal(expected, paginated.HasPrevious);
        }

        [Theory]
        [InlineData(1, 20, true)]
        [InlineData(4, 20, false)]
        public void HasNext_ShouldReturnExpectedValue(int page, int totalCount, bool expected)
        {
            var paginated = new Paginated<string>([], totalCount, page, 5);
            Assert.Equal(expected, paginated.HasNext);
        }

        [Fact]
        public void Create_ShouldReturnPaginatedInstanceWithCorrectData()
        {
            // Arrange
            var items = new[] { 1, 2, 3 };

            // Act
            var result = Paginated<int>.Create(items, totalCount: 3, page: 1, pageSize: 2);

            // Assert
            Assert.Equal(items, result.Items);
            Assert.Equal(3, result.TotalCount);
            Assert.Equal(1, result.Page);
            Assert.Equal(2, result.PageSize);
        }
    }
}
