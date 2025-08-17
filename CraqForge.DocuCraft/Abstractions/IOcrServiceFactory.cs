using CraqForge.DocuCraft.Abstractions.Ocr;
using Tesseract;

namespace CraqForge.DocuCraft.Abstractions
{
    public interface IOcrServiceFactory : IDisposable
    {
        public IOcrService Create(
             OcrAccuracy? accuracy = null,
             EngineMode? engineMode = null,
             Language? language = null,
             int? maxPoolSize = null);
    }
}
