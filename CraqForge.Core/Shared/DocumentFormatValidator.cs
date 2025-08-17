using System.IO.Compression;
using System.Text;

namespace CraqForge.Core.Shared
{
    public static class DocumentFormatValidator
    {
        private const string PdfSignature = "%PDF-";

        public static bool IsPdf(byte[] bytes)
        {
            if (bytes == null || bytes.Length < PdfSignature.Length)
                return false;

            var header = Encoding.ASCII.GetString(bytes, 0, PdfSignature.Length);
            return header.StartsWith(PdfSignature, StringComparison.Ordinal);
        }

        public static bool IsDocx(Stream stream)
        {
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
