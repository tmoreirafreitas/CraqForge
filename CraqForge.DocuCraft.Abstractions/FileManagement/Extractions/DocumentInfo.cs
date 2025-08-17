namespace CraqForge.DocuCraft.Abstractions.FileManagement.Extractions
{
    public record DocumentInfo
    {
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; init; } = string.Empty;
        public string Extension { get; init; } = string.Empty;
        public string Title { get; init; } = string.Empty;
        public string Author { get; init; } = string.Empty;
        public string Modifier { get; init; } = string.Empty;
        public string Subject { get; init; } = string.Empty;
        public string Keywords { get; init; } = string.Empty;
        public DateTime? CreationDate { get; init; }
        public DateTime? ModificationDate { get; init; }
        public DateTime ExtractionDate { get; set; }
    }
}
