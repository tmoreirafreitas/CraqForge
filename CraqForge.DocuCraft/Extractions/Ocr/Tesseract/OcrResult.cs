namespace CraqForge.DocuCraft.Extractions.Ocr.Tesseract
{
    public record OcrResult
    {
        public string Text { get; init; } = string.Empty;
        public float Confidence { get; init; }
    }
}
