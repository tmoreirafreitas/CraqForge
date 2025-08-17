using CraqForge.Core.Shared;
using CraqForge.DocuCraft.Abstractions.FileManagement.Extractions.Word;
using CraqForge.DocuCraft.Shared;
using Xceed.Words.NET;

namespace CraqForge.DocuCraft.Extractions.Word
{
    internal sealed class DocxExtractor : IDocxExtractor
    {
        private readonly DocX _document;
        private bool disposedValue;
        private static DateTime? ParseDate(string dateString)
        {
            if (DateTime.TryParse(dateString, out var date))
                return date;

            return null;
        }

        public DocxExtractor(byte[] docxContent)
        {
            if (docxContent == null || docxContent.Length == 0)
                throw new ArgumentException(DocumentValidationMessage.ContentIsEmpty);

            using var memoryStream = new MemoryStream(docxContent);

            if (!DocumentFormatValidator.IsDocx(memoryStream))
                throw new NotSupportedException(DocumentValidationMessage.SupportsOnlyDocx);

            // Resetar o stream para leitura do DocX após validação
            memoryStream.Position = 0;

            _document = DocX.Load(memoryStream);
        }

        public DocxInfo ExtractInfo()
        {
            var coreProps = _document.CoreProperties;

            coreProps.TryGetValue("dc:title", out var title);
            coreProps.TryGetValue("dc:subject", out var subject);
            coreProps.TryGetValue("dc:creator", out var author);
            coreProps.TryGetValue("cp:keywords", out var keywords);
            coreProps.TryGetValue("dc:description", out var description);
            coreProps.TryGetValue("dc:lastModifiedBy", out var modifier);
            coreProps.TryGetValue("cp:revision", out var version);
            coreProps.TryGetValue("dcterms:created", out var created);
            coreProps.TryGetValue("dcterms:modified", out var modified);

            return new DocxInfo
            {
                Title = title,
                Author = author,
                Modifier = modifier,
                Subject = subject,
                Keywords = keywords,
                Description = description,
                CreationDate = ParseDate(created),
                ModificationDate = ParseDate(modified),
                ApplicationVersion = version
            };
        }

        public string ExtractTextFromFile(byte[] docxBytes) => _document.Text;

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _document?.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
