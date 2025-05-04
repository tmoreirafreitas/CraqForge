using CraqForge.Core.Abstractions.FileManagement;
using CraqForge.Core.FileConversion;

namespace CraqForge.Core.Tests
{
    public class FormatConversionTests
    {
        private readonly FormatConversion _formatConversion = new();

        [Theory]
        [InlineData(FormatConversionType.Docx, "application/vnd.openxmlformats-officedocument.wordprocessingml.document")]
        [InlineData(FormatConversionType.Pdf, "application/pdf")]
        [InlineData(FormatConversionType.Undefined, "application/octet-stream")]
        [InlineData((FormatConversionType)999, "application/octet-stream")]
        public void GetContentType_ShouldReturnExpectedMimeType(FormatConversionType format, string expected)
        {
            var result = _formatConversion.GetContentType(format);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("document.docx", FormatConversionType.Docx)]
        [InlineData("presentation.ppt", FormatConversionType.Ppt)]
        [InlineData("archive.unknown", FormatConversionType.Undefined)]
        [InlineData("noextension", FormatConversionType.Undefined)]
        public void GetFormatType_ShouldReturnExpectedFormatType(string fileName, FormatConversionType expected)
        {
            var result = _formatConversion.GetFormatType(fileName);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(FormatConversionType.Xlsx, ".xlsx")]
        [InlineData(FormatConversionType.Tiff, ".tiff")]
        [InlineData(FormatConversionType.Undefined, null)]
        [InlineData((FormatConversionType)1234, null)]
        public void GetExtensionType_ShouldReturnExpectedExtension(FormatConversionType format, string? expected)
        {
            var result = _formatConversion.GetExtensionType(format);
            Assert.Equal(expected, result);
        }
    }
}
