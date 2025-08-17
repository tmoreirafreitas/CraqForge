using CraqForge.Core.Shared;
using CraqForge.DocuCraft.Abstractions;
using CraqForge.DocuCraft.Abstractions.FileManagement.Extractions.Pdf;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace CraqForge.DocuCraft.Extractions.Pdf
{
    internal sealed class PdfExtractor : IPdfExtractor, IDisposable
    {
        private readonly ILogger<PdfExtractor> _logger;
        private readonly MemoryStream _pdfStream;
        private readonly PdfReader _pdfReader;
        private readonly PdfDocument _document;

        private static readonly string[] ScanSignatures =
            ["scanned by", "camscanner", "genius scan", "adobe scan", "microsoft lens"];

        private bool _disposed;

        public PdfExtractor(byte[] pdfContent, ILogger<PdfExtractor>? logger = null)
        {
            _logger = logger ?? NullLogger<PdfExtractor>.Instance;
            ValidateContent(pdfContent);

            _pdfStream = new MemoryStream(pdfContent, writable: false);
            _pdfReader = new PdfReader(_pdfStream);
            _document = new PdfDocument(_pdfReader);

            _logger.LogInformation("PdfExtractor inicializado. Páginas: {Pages} | Versão PDF: {Version}",
                _document.GetNumberOfPages(), _document.GetPdfVersion());
        }

        private static void ValidateContent(byte[] pdfContent)
        {
            if (pdfContent is null)
                throw new ArgumentNullException(nameof(pdfContent), "O conteúdo do PDF não pode ser nulo.");
            if (pdfContent.Length == 0)
                throw new ArgumentException("O conteúdo do PDF está vazio.", nameof(pdfContent));
            if (!DocumentFormatValidator.IsPdf(pdfContent))
                throw new NotSupportedException("O arquivo informado não está em um formato PDF válido.");
        }

        public async Task<IReadOnlyList<PdfPageContent>> ExtractAllPagesAsImagesAsync(CancellationToken cancellationToken = default)
        {
            var sw = Stopwatch.StartNew();
            int pageCount = GetPageCount();
            _logger.LogInformation("Iniciando extração de {Count} páginas como imagens...", pageCount);

            var results = new PdfPageContent[pageCount];

            var tasks = Enumerable.Range(1, pageCount).Select(async i =>
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var imgBytes = await MergeImagesFromPageAsync(_document.GetPage(i), cancellationToken).ConfigureAwait(false);
                    results[i - 1] = new PdfPageContent { Page = i, ImageContent = imgBytes };

                    _logger.LogDebug("Página {Page} convertida em imagem ({Bytes} bytes).", i, imgBytes.Length);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogWarning("Extração de imagem cancelada na página {Page}.", i);
                    results[i - 1] = new PdfPageContent { Page = i, ImageContent = [] };
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Falha ao converter a página {Page} em imagem.", i);
                    results[i - 1] = new PdfPageContent { Page = i, ImageContent = [] };
                }
            });

            await Task.WhenAll(tasks).ConfigureAwait(false);
            sw.Stop();
            _logger.LogInformation("Extração de imagens concluída. Páginas: {Pages} | Tempo: {ElapsedMs} ms",
                pageCount, sw.ElapsedMilliseconds);

            return Array.AsReadOnly(results);
        }

        public async Task<byte[]> ExtractPageAsImageAsync(int pageNumber, CancellationToken cancellationToken = default)
        {
            int pageCount = _document.GetNumberOfPages();
            if (pageNumber < 1 || pageNumber > pageCount)
                throw new ArgumentOutOfRangeException(nameof(pageNumber),
                    $"A página {pageNumber} não existe no documento. Total: {pageCount}");

            _logger.LogInformation("Convertendo página {Page} em imagem...", pageNumber);
            var sw = Stopwatch.StartNew();
            try
            {
                var bytes = await MergeImagesFromPageAsync(_document.GetPage(pageNumber), cancellationToken).ConfigureAwait(false);
                sw.Stop();
                _logger.LogInformation("Página {Page} convertida. Tamanho: {Bytes} bytes | Tempo: {ElapsedMs} ms",
                    pageNumber, bytes.Length, sw.ElapsedMilliseconds);
                return bytes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao converter a página {Page} em imagem.", pageNumber);
                throw;
            }
        }

        /// <summary>
        /// Extrai as imagens embarcadas na página, empilha verticalmente e retorna um PNG em memória.
        /// Tudo em memória (sem arquivos temporários).
        /// </summary>
        private static async Task<byte[]> MergeImagesFromPageAsync(PdfPage page, CancellationToken cancellationToken)
        {
            var listener = new PdfImageExtractor();
            var processor = new PdfCanvasProcessor(listener);
            processor.ProcessPageContent(page);

            if (listener.Images.Count == 0)
                return [];

            var images = new List<Image<Rgba32>>();
            try
            {
                foreach (var imgData in listener.Images)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var image = Image.Load<Rgba32>(imgData);
                    images.Add(image);
                }

                int totalHeight = images.Sum(i => i.Height);
                int maxWidth = images.Max(i => i.Width);

                using var finalImage = new Image<Rgba32>(maxWidth, totalHeight);

                int offsetY = 0;
                foreach (var img in images)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    finalImage.Mutate(x => x.DrawImage(img, new Point(0, offsetY), 1f));
                    offsetY += img.Height;
                }

                await using var msOut = new MemoryStream();
                await finalImage.SaveAsync(msOut, new PngEncoder(), cancellationToken);
                return msOut.ToArray();
            }
            finally
            {
                foreach (var img in images)
                {
                    img.Dispose();
                }
            }
        }

        public Task<string> ExtractFullTextAsync(CancellationToken cancellationToken = default)
        {
            var sw = Stopwatch.StartNew();
            var output = new StringBuilder();
            int totalPages = _document.GetNumberOfPages();

            _logger.LogInformation("Iniciando extração de texto completo. Páginas: {Pages}", totalPages);

            for (int i = 1; i <= totalPages; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var page = _document.GetPage(i);
                var strategy = new LocationTextExtractionStrategy();
                var pageText = PdfTextExtractor.GetTextFromPage(page, strategy);

                pageText = pageText.Replace("\t", " ");
                output.AppendLine(pageText);
            }

            sw.Stop();
            _logger.LogInformation("Texto extraído. Tamanho: {Chars} chars | Tempo: {ElapsedMs} ms",
                output.Length, sw.ElapsedMilliseconds);

            return Task.FromResult(output.ToString());
        }

        public string ExtractPageAsText(int pageNumber)
        {
            int totalPages = _document.GetNumberOfPages();
            if (pageNumber < 1 || pageNumber > totalPages)
                throw new ArgumentOutOfRangeException(nameof(pageNumber),
                    $"A página {pageNumber} não existe no documento. Total: {totalPages}");

            _logger.LogDebug("Extraindo texto da página {Page}...", pageNumber);

            var page = _document.GetPage(pageNumber);
            var strategy = new LocationTextExtractionStrategy();
            var extractedText = PdfTextExtractor.GetTextFromPage(page, strategy);
            extractedText = extractedText.Replace("\t", " ");

            _logger.LogDebug("Texto extraído da página {Page}. Tamanho: {Len} chars", pageNumber, extractedText.Length);
            return extractedText;
        }

        public IReadOnlyList<PdfPageContent> ExtractSpecificPagesAsText(params int[] pageNumbers)
        {
            int totalPages = GetPageCount();
            var requestedPages = pageNumbers?.Distinct().OrderBy(p => p).ToArray() ?? Array.Empty<int>();

            _logger.LogInformation("Extraindo texto das páginas específicas: {Pages}", requestedPages);

            var invalidPages = requestedPages.Where(p => p < 1 || p > totalPages).ToList();
            if (invalidPages.Count != 0)
                throw new ArgumentOutOfRangeException(nameof(pageNumbers),
                    $"As seguintes páginas não existem (1 a {totalPages}): {string.Join(", ", invalidPages)}");

            var pagesContents = new List<PdfPageContent>(capacity: requestedPages.Length);
            foreach (var pageNumber in requestedPages)
            {
                var page = _document.GetPage(pageNumber);
                var strategy = new LocationTextExtractionStrategy();
                var text = PdfTextExtractor.GetTextFromPage(page, strategy);
                text = text.Replace("\t", " ");

                pagesContents.Add(new PdfPageContent { Page = pageNumber, Text = text });
            }

            _logger.LogInformation("Extração concluída para {Count} páginas específicas.", pagesContents.Count);
            return pagesContents.AsReadOnly();
        }

        public IReadOnlyList<PdfPageContent> ExtractTextByPage(CancellationToken cancellationToken = default)
        {
            int totalPages = _document.GetNumberOfPages();
            _logger.LogInformation("Extraindo texto por página. Total: {Pages}", totalPages);

            var pagesContents = new List<PdfPageContent>(capacity: totalPages);

            for (int i = 1; i <= totalPages; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var page = _document.GetPage(i);
                var strategy = new LocationTextExtractionStrategy();
                var text = PdfTextExtractor.GetTextFromPage(page, strategy);
                text = text.Replace("\t", " ");

                pagesContents.Add(new PdfPageContent { Page = i, Text = text });
            }

            _logger.LogInformation("Texto por página extraído. Páginas: {Pages}", pagesContents.Count);
            return pagesContents.AsReadOnly();
        }

        public PdfInfo ExtractInfo(string? fileName = "")
        {
            var info = _document.GetDocumentInfo();
            var pdfInfo = new PdfInfo
            {
                FileName = string.IsNullOrWhiteSpace(fileName) ? "Desconhecido" : fileName,
                FileSizeInBytes = _pdfReader.GetFileLength(),
                NumberOfPages = _document.GetNumberOfPages(),
                PdfVersion = _document.GetPdfVersion().ToString(),
                Title = info.GetTitle(),
                Author = info.GetAuthor(),
                Subject = info.GetSubject(),
                Keywords = info.GetKeywords(),
                Creator = info.GetCreator(),
                Producer = info.GetProducer(),
                CreationDate = ParseDate(info.GetMoreInfo("CreationDate")),
                ModificationDate = ParseDate(info.GetMoreInfo("ModDate")),
                IsPdfA = IsPdfA(_document),
                ExtractionDate = DateTime.Now
            };

            _logger.LogInformation("Metadados extraídos: Páginas={Pages}, Versão={Version}, PDF/A={PdfA}",
                pdfInfo.NumberOfPages, pdfInfo.PdfVersion, pdfInfo.IsPdfA);

            return pdfInfo;
        }

        public int GetPageCount() => _document.GetNumberOfPages();

        public bool PageContainsOnlyImages(int pageNumber)
        {
            var resources = _document.GetPage(pageNumber)?.GetResources();
            var xObjects = resources?.GetResource(PdfName.XObject);

            if (xObjects is null) return false;

            foreach (var key in xObjects.KeySet())
            {
                var stream = xObjects.GetAsStream(key);
                if (stream is PdfStream pdfStream)
                {
                    var subtype = pdfStream.GetAsName(PdfName.Subtype);
                    if (PdfName.Image.Equals(subtype))
                        return true;
                }
            }

            return false;
        }

        public bool IsTextLikelyFromScanner(string extractedText)
        {
            if (string.IsNullOrWhiteSpace(extractedText))
                return true;

            var words = extractedText
                .Trim()
                .Split([' ', '\n', '\r', '\t'], StringSplitOptions.RemoveEmptyEntries);

            return words.Length <= 5 &&
                   ScanSignatures.Any(sig =>
                       extractedText.Contains(sig, StringComparison.OrdinalIgnoreCase));
        }

        public bool PageNeedsOcr(int pageNumber)
        {
            var strategy = new SimpleTextExtractionStrategy();
            var extractedText = PdfTextExtractor.GetTextFromPage(_document.GetPage(pageNumber), strategy);

            return string.IsNullOrWhiteSpace(extractedText)
                   || PageContainsOnlyImages(pageNumber)
                   || IsTextLikelyFromScanner(extractedText);
        }

        private static DateTime? ParseDate(string pdfDate)
        {
            if (string.IsNullOrEmpty(pdfDate)) return null;

            try
            {
                if (pdfDate.StartsWith("D:"))
                    pdfDate = pdfDate[2..];

                var formats = new[]
                {
                    "yyyyMMddHHmmssK",
                    "yyyyMMddHHmmss",
                    "yyyyMMdd"
                };

                foreach (var fmt in formats)
                {
                    if (DateTime.TryParseExact(pdfDate, fmt, CultureInfo.InvariantCulture,
                        DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out var dt))
                        return dt;
                }
            }
            catch
            {
                // parsing inválido ignorado
            }

            return null;
        }

        private static bool IsPdfA(PdfDocument doc)
        {
            var outputIntents = doc.GetCatalog().GetPdfObject()?.GetAsArray(PdfName.OutputIntents);
            if (outputIntents == null) return false;

            foreach (var item in outputIntents)
            {
                if (item is PdfDictionary dict)
                {
                    var s = dict.GetAsName(PdfName.S);
                    if (s != null && s.Equals(PdfName.GTS_PDFA1))
                        return true;

                    var subtype = dict.GetAsName(PdfName.Subtype);
                    if (subtype != null && subtype.GetValue().Contains("PDF/A", StringComparison.OrdinalIgnoreCase))
                        return true;
                }
            }

            return false;
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                try { _document?.Close(); } catch { /* swallow */ }
                try { _pdfReader?.Close(); } catch { /* swallow */ }
                try { _pdfStream?.Dispose(); } catch { /* swallow */ }
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
