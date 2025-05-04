using CraqForge.Core.Abstractions.Validations;
using System.IO.Compression;
using System.Text;

namespace CraqForge.Core.Shared
{
    internal sealed class DocumentFormatValidator : IDocumentFormatValidator
    {
        private const string PdfSignature = "%PDF-";

        public bool IsPdf(byte[] pdfBytes)
        {
            if (pdfBytes == null || pdfBytes.Length < PdfSignature.Length)
                return false;

            var header = Encoding.ASCII.GetString(pdfBytes, 0, PdfSignature.Length);
            return header.StartsWith(PdfSignature, StringComparison.Ordinal);
        }

        public bool IsDocx(byte[] docxBytes)
        {
            if(docxBytes == null || docxBytes.Length == 0) 
                return false;

            var stream = new MemoryStream(docxBytes);
            if (stream == null || !stream.CanRead)
                return false;

            try
            {
                // Salva a posição atual para restaurar depois
                long originalPosition = stream.CanSeek ? stream.Position : 0;

                using var archive = new ZipArchive(stream, ZipArchiveMode.Read, leaveOpen: true);
                bool hasDocumentXml = archive.Entries.Any(entry =>
                    entry.FullName.Equals("word/document.xml", StringComparison.OrdinalIgnoreCase));

                // Restaura a posição original do stream
                if (stream.CanSeek)
                    stream.Position = originalPosition;

                return hasDocumentXml;
            }
            catch
            {
                return false;
            }
        }
    }
}
