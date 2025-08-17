using CraqForge.DocuCraft.Extractions.Ocr.Tesseract;

namespace CraqForge.DocuCraft.Abstractions
{
    public interface IOcrService : IDisposable
    {
        OcrResult ExtractText(byte[] imageBytes);
        Task<Dictionary<string, OcrResult>> ExtractTextFromMultipleAsync(Dictionary<string, byte[]> imagesByKey);
    }
}
