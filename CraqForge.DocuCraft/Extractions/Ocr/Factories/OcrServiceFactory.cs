using CraqForge.DocuCraft.Abstractions.Ocr;
using CraqForge.DocuCraft.Abstractions;
using CraqForge.DocuCraft.Extractions.Ocr.Tesseract;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using Tesseract;

internal sealed class OcrServiceFactory(ILoggerFactory loggerFactory) : IOcrServiceFactory
{
    private readonly ILoggerFactory _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
    private readonly ConcurrentDictionary<string, PoolEntry> _enginePools = new();
    private bool _disposed;

    private sealed class PoolEntry(int maxSize)
    {
        public readonly ConcurrentBag<TesseractEngine> Bag = new();
        private int _count;
        private readonly int _maxSize = Math.Max(1, maxSize);

        public bool IncrementIfBelowMax()
        {
            while (true)
            {
                int current = _count;
                if (current >= _maxSize) return false;
                if (Interlocked.CompareExchange(ref _count, current + 1, current) == current)
                    return true;
            }
        }
    }

    public string DefaultTessdataPath { get; } = OcrDataManager.GetTessdataPath();
    public OcrAccuracy DefaultAccuracy { get; } = OcrAccuracy.Standard;
    public EngineMode DefaultEngineMode { get; } = EngineMode.Default;
    public Language DefaultLanguage { get; } = Language.Por;
    public int? DefaultMaxPoolSize { get; } = 5;

    public IOcrService Create(
         OcrAccuracy? accuracy = null,
         EngineMode? engineMode = null,
         Language? language = null,
         int? maxPoolSize = null)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(OcrServiceFactory));

        accuracy ??= DefaultAccuracy;
        engineMode ??= DefaultEngineMode;
        language ??= DefaultLanguage;

        string trainedData = GetTrainedData(language.Value, accuracy.Value);
        string fullPath = Path.Combine(DefaultTessdataPath, trainedData + ".traineddata");

        if (!File.Exists(fullPath))
            throw new FileNotFoundException($"Arquivo de treinamento não encontrado: {fullPath}");

        var loggerFactoryLog = _loggerFactory.CreateLogger<OcrServiceFactory>();
        loggerFactoryLog.LogInformation(
            "Criando OcrService: tessdata={Path}, trainedData={Trained}, accuracy={Accuracy}, engineMode={Mode}, language={Lang}, usePooling={UsePooling}",
            DefaultTessdataPath, trainedData, accuracy.Value, engineMode.Value, language.Value, maxPoolSize != null && maxPoolSize.Value > 0);

        TesseractEngine engine;
        bool ownsEngine;

        string poolKey = $"{DefaultTessdataPath}|{trainedData}|{engineMode}";
        if (maxPoolSize.HasValue)
        {
            var poolEntry = _enginePools.GetOrAdd(poolKey, _ => new PoolEntry(maxPoolSize.Value));

            if (!poolEntry.Bag.TryTake(out engine))
            {
                if (poolEntry.IncrementIfBelowMax())
                {
                    loggerFactoryLog.LogDebug("Criando nova engine para o pool {PoolKey}", poolKey);
                    engine = new TesseractEngine(DefaultTessdataPath, trainedData, engineMode.Value);
                }
                else
                {
                    throw new InvalidOperationException($"Limite de engines simultâneas atingido para pool '{poolKey}'.");
                }
            }
            else
            {
                loggerFactoryLog.LogDebug("Reutilizando engine do pool {PoolKey}", poolKey);
            }

            ownsEngine = false; // engine será devolvida ao pool
        }
        else
        {
            engine = new TesseractEngine(DefaultTessdataPath, trainedData, engineMode.Value);
            ownsEngine = true;
        }

        var loggerOcr = _loggerFactory.CreateLogger<OcrService>();
        return new OcrService(engine, loggerOcr, this, ownsEngine, !ownsEngine ? poolKey : string.Empty);
    }

    public void ReturnEngineToPool(string poolKey, TesseractEngine engine)
    {
        if (_disposed)
        {
            engine.Dispose();
            return;
        }

        if (_enginePools.TryGetValue(poolKey, out var poolEntry))
        {
            poolEntry.Bag.Add(engine);
        }
        else
        {
            engine.Dispose();
        }
    }

    private static string GetTrainedData(Language language, OcrAccuracy accuracy)
    {
        string code = language.ToString().ToLowerInvariant();
        return accuracy switch
        {
            OcrAccuracy.Standard => code,
            OcrAccuracy.Fast => $"{code}.fast",
            OcrAccuracy.Best => $"{code}.best",
            _ => code
        };
    }

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                foreach (var kvp in _enginePools)
                {
                    while (kvp.Value.Bag.TryTake(out var engine))
                    {
                        try
                        {
                            engine.Dispose();
                        }
                        catch { /* ignorar falhas no dispose */ }
                    }
                }

                _enginePools.Clear();
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
