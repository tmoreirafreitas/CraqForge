using CraqForge.DocuCraft.Abstractions.FileManagement.Conversions;
using Microsoft.Extensions.Logging;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using System.Runtime.InteropServices;

namespace CraqForge.DocuCraft.Conversion
{
    public class HtmlToPdfConverter(ILogger<HtmlToPdfConverter> logger) : IHtmlToPdfConverter
    {
        public async Task<byte[]> ConvertTextHtmlToPdfAsync(string htmlContent, CancellationToken cancellation = default)
        {
            return await ToPdfAsync(htmlContent, logger, cancellation);
        }

        public async Task<byte[]> ConvertTextHtmlToPdfAsync(byte[] fileBytes, CancellationToken cancellation = default)
        {
            using var ms = new MemoryStream(fileBytes);
            using var reader = new StreamReader(ms);

            string htmlContent = await reader.ReadToEndAsync(cancellation);

            return await ToPdfAsync(htmlContent, logger, cancellation);
        }

        private static async Task<byte[]> ToPdfAsync(string textHtml, ILogger? logger = null, CancellationToken cancellation = default)
        {
            string chromiumExecutablePath = GetChromiumExecutablePath();
            if (string.IsNullOrEmpty(chromiumExecutablePath))
            {
                logger?.LogInformation("Starting Chromium download...");
                await new BrowserFetcher().DownloadAsync();
                chromiumExecutablePath = null;
            }
            else
            {
                logger?.LogInformation("Using local Chromium: {ChromiumExecutablePath}", chromiumExecutablePath);
            }

            using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                ExecutablePath = chromiumExecutablePath,
                Args = ["--no-sandbox", "--disable-setuid-sandbox"]
            });
            logger?.LogInformation("Chromium started.");

            using var page = await browser.NewPageAsync();
            await page.SetContentAsync(textHtml);
            logger?.LogInformation("HTML loaded on the virtual page.");

            var pdfBytes = await page.PdfDataAsync(new PdfOptions
            {
                Format = PaperFormat.A4,
                PrintBackground = true,
                MarginOptions = new MarginOptions
                {
                    Top = "2.5cm",
                    Bottom = "2.0cm",
                    Left = "2.5cm",
                    Right = "2.5cm"
                }
            });

            await browser.CloseAsync();
            logger?.LogInformation("PDF generated successfully.");

            return pdfBytes;
        }

        private static string? GetChromiumExecutablePath()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                if (File.Exists("/usr/bin/chromium"))
                    return "/usr/bin/chromium";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                if (File.Exists("/Applications/Chromium.app/Contents/MacOS/Chromium"))
                    return "/Applications/Chromium.app/Contents/MacOS/Chromium";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                string chromiumPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Chromium\\chrome.exe");
                if (File.Exists(chromiumPath))
                    return chromiumPath;

                chromiumPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Chromium\\chrome.exe");
                if (File.Exists(chromiumPath))
                    return chromiumPath;
            }

            return null;
        }
    }
}