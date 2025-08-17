using CraqForge.DocuCraft.Abstractions;
using CraqForge.DocuCraft.Extractions.Ocr.Tesseract;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using Tesseract;

internal sealed class OcrService : IOcrService
{
    private readonly ILogger<OcrService> _logger;
    private readonly OcrServiceFactory _factory;    
    private readonly TesseractEngine _engine;
    private readonly bool _ownsEngine;
    private readonly string? _poolKey = string.Empty;
    private bool _disposed;

    internal OcrService(TesseractEngine engine, ILogger<OcrService> logger, OcrServiceFactory factory, bool ownsEngine = true, string poolKey = "")
    {
        _engine = engine ?? throw new ArgumentNullException(nameof(engine));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        _ownsEngine = ownsEngine;
        _poolKey = poolKey;
    }

    public OcrResult ExtractText(byte[] imageBytes)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(OcrService));

        try
        {
            using var img = Pix.LoadFromMemory(imageBytes);

            using (var page = _engine.Process(img, PageSegMode.SingleBlock))
            {
                var text = page.GetText();
                if (!string.IsNullOrWhiteSpace(text))
                    return new OcrResult { Text = text, Confidence = page.GetMeanConfidence() };
            }

            using var fallbackPage = _engine.Process(img, PageSegMode.SparseTextOsd);
            return new OcrResult { Text = fallbackPage.GetText(), Confidence = fallbackPage.GetMeanConfidence() };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante OCR com Tesseract.");
            throw new TesseractException("Falha na extração de texto com OCR Tesseract.", ex);
        }
    }

    /// <summary>
    /// Processa múltiplas imagens em paralelo, pegando engines do pool automaticamente.
    /// </summary>
    public async Task<Dictionary<string, OcrResult>> ExtractTextFromMultipleAsync(Dictionary<string, byte[]> imagesByKey)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(OcrService));
        var result = new ConcurrentDictionary<string, OcrResult>();

        var tasks = imagesByKey.Select(kvp => Task.Run(() =>
        {
            // Cria um OcrService independente para cada imagem usando a factory
            using var service = _factory.Create(
                accuracy: _factory.DefaultAccuracy,
                engineMode: _factory.DefaultEngineMode,
                language: _factory.DefaultLanguage,
                maxPoolSize: _factory.DefaultMaxPoolSize);

            try
            {
                var extraction = service.ExtractText(kvp.Value);
                result.TryAdd(kvp.Key, extraction);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Falha ao processar imagem '{Key}'", kvp.Key);
                throw;
            }
        }));

        await Task.WhenAll(tasks);
        return result.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                if (_ownsEngine)
                {
                    _logger.LogDebug("Descartando TesseractEngine associado a OcrService.");
                    _engine?.Dispose();
                }
                else
                {
                    if (!string.IsNullOrEmpty(_poolKey))
                        _factory.ReturnEngineToPool(_poolKey, _engine);
                }
            }

            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}