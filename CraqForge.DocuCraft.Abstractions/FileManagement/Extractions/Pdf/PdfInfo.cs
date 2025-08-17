namespace CraqForge.DocuCraft.Abstractions.FileManagement.Extractions.Pdf
{
    public record PdfInfo : DocumentInfo
    {
        public long FileSizeInBytes { get; set; }
        public int NumberOfPages { get; set; }
        public string PdfVersion { get; set; } = string.Empty;
        public string Creator { get; set; } = string.Empty;
        public string Producer { get; set; } = string.Empty;
        public bool IsPdfA { get; set; }
        public bool HasXmpMetadata { get; set; }
    }
}
