namespace CraqForge.DocuCraft.Abstractions.FileManagement.Extractions.Word
{
    public record DocxInfo : DocumentInfo
    {
        public string Description { get; set; } = string.Empty;
        public string ApplicationVersion { get; set; } = string.Empty;
    }
}
