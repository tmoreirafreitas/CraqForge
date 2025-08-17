using CraqForge.Core.Abstractions.FileManagement;
using CraqForge.Core.Extensions;
using CraqForge.DocuCraft.Abstractions.FileManagement;
using CraqForge.DocuCraft.Abstractions.FileManagement.Conversions;
using CraqForge.DocuCraft.Shared;
using Microsoft.Extensions.Logging;

namespace CraqForge.DocuCraft.Conversion
{
    internal sealed class LibreOfficeConverter(ILogger<LibreOfficeConverter> logger, 
        ILibreOfficePathResolver libreOfficePathResolver,
        IFormatConversion formatConversion) : ILibreOfficeConverter
    {
        private readonly LibreOfficeExecutor _executor = new(logger);

        public async Task<string> ConvertAsync(string inputFile, string outputDir,
            FormatConversionType format = FormatConversionType.Pdf,
            CancellationToken cancellation = default)
        {
            if (!File.Exists(inputFile))
                throw new FileNotFoundException($"Input file not found: {inputFile}");

            Directory.CreateDirectory(outputDir);

            var formatDescription = format.GetEnumMemberValue();
            var sofficePath = libreOfficePathResolver.GetSofficePath();
            var useDocker = libreOfficePathResolver.ShouldUseDocker;
            var dockerCommand = libreOfficePathResolver.GetDockerCommand(inputFile, outputDir, formatDescription);
            var arguments = $"--headless --convert-to {formatDescription} \"{inputFile}\" --outdir \"{outputDir}\"";

            logger?.LogInformation("Iniciando conversão de {InputFile} para {Format}.", inputFile, formatDescription);

            int exitCode = await _executor.ExecuteAsync(
                sofficePath,
                arguments,
                useDocker,
                dockerCommand,
                cancellation
            );

            if (exitCode != 0)
                throw new InvalidOperationException($"Falha ao converter arquivo. Código de saída: {exitCode}");

            string convertedFile = Path.Combine(outputDir,
                Path.ChangeExtension(Path.GetFileNameWithoutExtension(inputFile),
                formatConversion.GetExtensionType(format)));

            if (!File.Exists(convertedFile))
                throw new FileNotFoundException("Converted file not found.", convertedFile);

            logger?.LogInformation("Conversão concluída com sucesso. Arquivo gerado: {ConvertedFile}", convertedFile);

            return convertedFile;
        }

        public async Task<byte[]> ConvertToByteArrayAsync(string inputFile, string outputDir,
            FormatConversionType format = FormatConversionType.Pdf,
            CancellationToken cancellation = default)
        {
            string convertedFile = await ConvertAsync(inputFile, outputDir, format, cancellation);
            return await File.ReadAllBytesAsync(convertedFile, cancellation);
        }

        public async Task<Stream> ConvertToMemoryStreamAsync(string inputFile, string outputDir,
            FormatConversionType format = FormatConversionType.Pdf,
            CancellationToken cancellation = default)
        {
            string convertedFile = await ConvertAsync(inputFile, outputDir, format, cancellation);

            if (!File.Exists(convertedFile))
                throw new FileNotFoundException("Converted file not found.", convertedFile);

            logger?.LogInformation("Lendo arquivo convertido para MemoryStream: {ConvertedFile}", convertedFile);

            var memoryStream = new MemoryStream();
            await using (var fileStream = new FileStream(convertedFile, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true))
            {
                await fileStream.CopyToAsync(memoryStream, 81920, cancellation);
            }

            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }
    }
}