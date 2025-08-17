namespace CraqForge.Core.Abstractions.FileManagement.Models
{
    public record TextFormatted
    {
        public string Text { get; set; } = string.Empty;
        public bool IsBold { get; set; }
        public bool IsItalic { get; set; }
    }
}
