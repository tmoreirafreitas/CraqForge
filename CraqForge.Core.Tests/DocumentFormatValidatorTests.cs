using CraqForge.Core.Shared;
using System.IO.Compression;
using System.Text;

namespace CraqForge.Core.Tests
{
    public class DocumentFormatValidatorTests
    {
        private readonly DocumentFormatValidator _documentFormatValidator = new();

        [Fact]
        public void IsPdf_ShouldReturnTrueForValidPdf()
        {
            // Criando um PDF simples com a assinatura "%PDF-"
            var pdfBytes = Encoding.ASCII.GetBytes("%PDF-12345");

            var result = _documentFormatValidator.IsPdf(pdfBytes);

            Assert.True(result);
        }

        [Fact]
        public void IsPdf_ShouldReturnFalseForInvalidPdf()
        {
            // Criando um arquivo com bytes que não correspondem à assinatura de PDF
            var invalidPdfBytes = Encoding.ASCII.GetBytes("Not a PDF file");

            var result = _documentFormatValidator.IsPdf(invalidPdfBytes);

            Assert.False(result);
        }

        [Fact]
        public void IsPdf_ShouldReturnFalseForNullBytes()
        {
            byte[] nullBytes = null;

            var result = _documentFormatValidator.IsPdf(nullBytes);

            Assert.False(result);
        }

        [Fact]
        public void IsPdf_ShouldReturnFalseForEmptyBytes()
        {
            var emptyBytes = new byte[0];

            var result = _documentFormatValidator.IsPdf(emptyBytes);

            Assert.False(result);
        }

        [Fact]
        public void IsDocx_ShouldReturnTrueForValidDocx()
        {
            // Criando um arquivo DOCX válido
            var docxBytes = CreateValidDocxFile();

            var result = _documentFormatValidator.IsDocx(docxBytes);

            Assert.True(result);
        }

        [Fact]
        public void IsDocx_ShouldReturnFalseForInvalidDocx()
        {
            // Criando um arquivo com bytes inválidos
            var invalidDocxBytes = Encoding.ASCII.GetBytes("Invalid DOCX content");

            var result = _documentFormatValidator.IsDocx(invalidDocxBytes);

            Assert.False(result);
        }

        [Fact]
        public void IsDocx_ShouldReturnFalseForNullBytes()
        {
            byte[] nullBytes = null;

            var result = _documentFormatValidator.IsDocx(nullBytes);

            Assert.False(result);
        }

        [Fact]
        public void IsDocx_ShouldReturnFalseForEmptyBytes()
        {
            var emptyBytes = new byte[0];

            var result = _documentFormatValidator.IsDocx(emptyBytes);

            Assert.False(result);
        }

        private byte[] CreateValidDocxFile()
        {
            // Criando um arquivo DOCX simples (em memória) com a entrada necessária
            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, leaveOpen: true))
                {
                    var documentEntry = archive.CreateEntry("word/document.xml", CompressionLevel.Optimal);
                    using (var entryStream = documentEntry.Open())
                    using (var writer = new StreamWriter(entryStream))
                    {
                        writer.Write("<xml></xml>");
                    }
                }
                return memoryStream.ToArray();
            }
        }
    }
}
