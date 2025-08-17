using CraqForge.Core.Shared;
using System.IO.Compression;
using System.Text;

namespace CraqForge.Core.Tests
{
    public class DocumentFormatValidatorTests
    {
        [Fact]
        public void IsPdf_ShouldReturnTrueForValidPdf()
        {
            // Criando um PDF simples com a assinatura "%PDF-"
            var pdfBytes = Encoding.ASCII.GetBytes("%PDF-12345");

            var result = DocumentFormatValidator.IsPdf(pdfBytes);

            Assert.True(result);
        }

        [Fact]
        public void IsPdf_ShouldReturnFalseForInvalidPdf()
        {
            // Criando um arquivo com bytes que não correspondem à assinatura de PDF
            var invalidPdfBytes = Encoding.ASCII.GetBytes("Not a PDF file");

            var result = DocumentFormatValidator.IsPdf(invalidPdfBytes);

            Assert.False(result);
        }

        [Fact]
        public void IsPdf_ShouldReturnFalseForNullBytes()
        {
            byte[] nullBytes = null;

            var result = DocumentFormatValidator.IsPdf(nullBytes);

            Assert.False(result);
        }

        [Fact]
        public void IsPdf_ShouldReturnFalseForEmptyBytes()
        {
            var emptyBytes = new byte[0];

            var result = DocumentFormatValidator.IsPdf(emptyBytes);

            Assert.False(result);
        }

        [Fact]
        public void IsDocx_ShouldReturnTrueForValidDocx()
        {
            // Criando um arquivo DOCX válido
            var docxBytes = CreateValidDocxFile();
            using var ms = new MemoryStream(docxBytes);
            var result = DocumentFormatValidator.IsDocx(ms);

            Assert.True(result);
        }

        [Fact]
        public void IsDocx_ShouldReturnFalseForInvalidDocx()
        {
            // Criando um arquivo com bytes inválidos
            var invalidDocxBytes = Encoding.ASCII.GetBytes("Invalid DOCX content");
            using var ms = new MemoryStream(invalidDocxBytes);
            var result = DocumentFormatValidator.IsDocx(ms);

            Assert.False(result);
        }

        [Fact]
        public void IsDocx_ShouldReturnFalseForNullBytes()
        {
            byte[] nullBytes = null;
            using var ms = new MemoryStream(nullBytes);
            var result = DocumentFormatValidator.IsDocx(ms);

            Assert.False(result);
        }

        [Fact]
        public void IsDocx_ShouldReturnFalseForEmptyBytes()
        {
            var emptyBytes = new byte[0];
            using var ms = new MemoryStream(emptyBytes);
            var result = DocumentFormatValidator.IsDocx(ms);

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
