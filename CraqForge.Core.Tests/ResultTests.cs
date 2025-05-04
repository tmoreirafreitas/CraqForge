using CraqForge.Core.Results;

namespace CraqForge.Core.Tests
{
    public class ResultTests
    {
        [Fact]
        public void Success_ShouldReturnSuccessfulResult()
        {
            // Act
            var result = Result.Success();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.False(result.IsFailure);
            Assert.Null(result.Error);
        }

        [Fact]
        public void Failure_ShouldReturnFailedResult_WithError()
        {
            // Arrange
            var errorMessage = "An error occurred.";

            // Act
            var result = Result.Failure(errorMessage);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.True(result.IsFailure);
            Assert.Equal(errorMessage, result.Error);
        }

        [Fact]
        public void Failure_ShouldAllowNullError()
        {
            // Act
            var result = Result.Failure(null);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.True(result.IsFailure);
            Assert.Null(result.Error);
        }
    }
}
