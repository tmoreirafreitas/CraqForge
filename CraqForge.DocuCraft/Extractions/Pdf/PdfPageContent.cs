using CraqForge.DocuCraft.Abstractions.FileManagement.Extractions.Pdf;

namespace CraqForge.DocuCraft.Extractions.Pdf
{
    public record PdfPageContent : IPdfPageContent
    {
        public int Page { get; init; }
        public string? Text { get; init; }
        public byte[]? ImageContent { get; init; }
    }
}
