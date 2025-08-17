namespace CraqForge.DocuCraft.Extractions.Ocr.Tesseract
{
    using CraqForge.DocuCraft.Abstractions.Ocr;
    using Microsoft.Extensions.Logging;
    using System.Net.Http;

    /// <summary>
    /// Gerencia os arquivos de dados do Tesseract (.traineddata), garantindo que os modelos
    /// de idioma em diferentes níveis de acurácia estejam disponíveis localmente.
    /// </summary>
    public class OcrDataManager
    {
        private static readonly string _tessdataPath = Path.Combine(Directory.GetCurrentDirectory(), "tesseract/tessdata");
        private static readonly string[] AccuracyLevels = { "standard", "fast", "best" };
        private static readonly string BaseUrl = "https://github.com/tesseract-ocr/tessdata{0}/raw/main/por.traineddata";
        private readonly ILogger _logger;

        /// <summary>
        /// Inicializa o gerenciador de dados do Tesseract e garante a existência do diretório `tessdata`.
        /// </summary>
        public OcrDataManager(ILogger<OcrDataManager> logger)
        {
            if (!Directory.Exists(_tessdataPath))
                Directory.CreateDirectory(_tessdataPath);

            _logger = logger;
        }

        /// <summary>
        /// Retorna o caminho absoluto do diretório onde os arquivos `.traineddata` são armazenados.
        /// </summary>
        /// <returns>Caminho completo da pasta `tessdata`.</returns>
        public static string GetTessdataPath() => _tessdataPath;

        /// <summary>
        /// Garante que todos os arquivos de modelo de idioma em português estejam presentes localmente.
        /// Faz o download automático dos arquivos caso estejam ausentes.
        /// Os níveis de acurácia cobertos são: padrão (standard), rápido (fast) e com melhor qualidade (best).
        /// </summary>
        public async Task EnsureLanguageModelsAsync(params Language[] languages)
        {
            var fileName = string.Empty;
            try
            {
                foreach (var language in languages)
                {
                    var languageCode = language.ToString().ToLower();
                    foreach (var accuracy in AccuracyLevels)
                    {
                        string? subPath = accuracy == "standard" ? "" : $"_{accuracy}";
                        string? url = string.Format(BaseUrl, subPath);
                        fileName = accuracy == "standard" ? $"{languageCode}.traineddata" : $"{languageCode}.{accuracy}.traineddata";
                        string? fullPath = Path.Combine(_tessdataPath, fileName);
                        if (!File.Exists(fullPath))
                        {
                            using var httpClient = new HttpClient();
                            var data = await httpClient.GetByteArrayAsync(url);
                            await File.WriteAllBytesAsync(fullPath, data);

                            _logger.LogInformation($"Baixado: {fileName}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Houve erro ao baixar o arquivo: {fileName}");
                throw;
            }
        }
    }
}
