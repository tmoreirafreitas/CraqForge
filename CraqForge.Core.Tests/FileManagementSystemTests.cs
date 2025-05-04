using CraqForge.Core.FileManagement;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text;

namespace CraqForge.Core.Tests
{
    public class FileManagementSystemTests
    {
        private readonly Mock<ILoggerFactory> _loggerFactoryMock;
        private readonly Mock<ILogger> _loggerMock;

        private readonly FileManagementSystem _fileManagementSystem;

        public FileManagementSystemTests()
        {
            _loggerFactoryMock = new Mock<ILoggerFactory>();
            _loggerMock = new Mock<ILogger>();
            _loggerFactoryMock
               .Setup(f => f.CreateLogger(It.IsAny<string>()))
               .Returns(_loggerMock.Object);
            _fileManagementSystem = new FileManagementSystemFake(_loggerFactoryMock.Object);
        }

        [Fact]
        public async Task DownloadAsync_ShouldReturnFileBytes_WhenFileExists()
        {
            // Arrange
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "testFile.txt");
            var expectedContent = "This is a test file.";
            await File.WriteAllTextAsync(filePath, expectedContent);

            // Act
            var result = await _fileManagementSystem.DownloadAsync(filePath);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedContent, Encoding.UTF8.GetString(result));
        }

        [Fact]
        public async Task DownloadAsync_ShouldThrowFileNotFoundException_WhenFileDoesNotExist()
        {
            // Arrange
            var invalidFilePath = "invalidFilePath.txt";

            // Act & Assert
            var exception = await Assert.ThrowsAsync<FileNotFoundException>(() =>
                _fileManagementSystem.DownloadAsync(invalidFilePath));

            Assert.Equal($"Arquivo não encontrado: {invalidFilePath}", exception.Message);
        }

        [Fact]
        public void CreateFolder_ShouldCreateFolder_WhenFolderDoesNotExist()
        {
            // Arrange
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "newFolder");

            // Act
            var result = _fileManagementSystem.CreateFolder(folderPath);

            // Assert
            Assert.True(Directory.Exists(result));
        }

        [Fact]
        public void CreateFolder_ShouldReturnExistingFolder_WhenFolderAlreadyExists()
        {
            // Arrange
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "existingFolder");
            Directory.CreateDirectory(folderPath);

            // Act
            var result = _fileManagementSystem.CreateFolder(folderPath);

            // Assert
            Assert.True(Directory.Exists(result));
        }

        [Fact]
        public void NewTempFileName_ShouldReturnUniqueFileName()
        {
            // Arrange
            var tempPath = Path.Combine(Directory.GetCurrentDirectory(), "Temp");

            // Act
            var result = _fileManagementSystem.NewTempFileName(tempPath);
            var fileName = Path.GetFileName(result);
            // Assert
            Assert.False(string.IsNullOrWhiteSpace(fileName));
            Assert.Matches(@"[A-F0-9]{32}", fileName); // Verifica se o GUID está no formato correto
        }

        [Fact]
        public async Task SaveFileAsync_ShouldSaveFile_WhenValidBytesProvided()
        {
            // Arrange
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "savedFile.txt");
            var content = Encoding.UTF8.GetBytes("File content");

            // Act
            await _fileManagementSystem.SaveFileAsync(content, filePath);

            // Assert
            Assert.True(File.Exists(filePath));
            Assert.Equal("File content", await File.ReadAllTextAsync(filePath));
        }

        [Fact]
        public async Task SaveFileAsync_ShouldThrowException_WhenBytesAreEmpty()
        {
            // Arrange
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "emptyFile.txt");

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                _fileManagementSystem.SaveFileAsync(new byte[0], filePath));

            Assert.Equal("O array de bytes está vazio.", exception.Message);
        }

        // Classe de teste para a implementação abstrata
        private class FileManagementSystemFake(ILoggerFactory loggerFactory) : FileManagementSystem(loggerFactory)
        {
        }
    }
}
