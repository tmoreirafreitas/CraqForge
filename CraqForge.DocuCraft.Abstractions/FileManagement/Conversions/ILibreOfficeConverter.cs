using CraqForge.Core.Abstractions.FileManagement;

namespace CraqForge.DocuCraft.Abstractions.FileManagement.Conversions
{
    /// <summary>
    /// Interface responsável por conversões de documentos utilizando o LibreOffice em modo headless.
    /// Suporta múltiplos formatos de saída e oferece resultados como caminho de arquivo, array de bytes ou stream.
    /// </summary>
    public interface ILibreOfficeConverter
    {
        /// <summary>
        /// Converte um arquivo para o formato especificado e salva o resultado no diretório informado.
        /// </summary>
        /// <param name="inputFile">Caminho completo do arquivo de entrada.</param>
        /// <param name="outputDir">Diretório onde o arquivo convertido será salvo.</param>
        /// <param name="format">Formato de conversão desejado.</param>
        /// <param name="logger">Logger utilizado para registrar informações e erros da conversão.</param>
        /// <param name="cancellation">Token opcional para cancelamento da operação.</param>
        /// <returns>Caminho completo do arquivo convertido.</returns>
        /// <exception cref="FileNotFoundException">Lançada se o arquivo de entrada não for encontrado.</exception>
        /// <exception cref="Exception">Lançada em caso de falha durante a conversão.</exception>
        Task<string> ConvertAsync(string inputFile, string outputDir, FormatConversionType format, CancellationToken cancellation = default);

        /// <summary>
        /// Converte um arquivo para o formato especificado e retorna o conteúdo convertido como um array de bytes.
        /// </summary>
        /// <param name="inputFile">Caminho completo do arquivo de entrada.</param>
        /// <param name="outputDir">Diretório onde o arquivo convertido será salvo temporariamente.</param>
        /// <param name="format">Formato de conversão desejado.</param>
        /// <param name="logger">Logger utilizado para registrar informações e erros da conversão.</param>
        /// <param name="cancellation">Token opcional para cancelamento da operação.</param>
        /// <returns>Conteúdo convertido do arquivo como array de bytes.</returns>
        Task<byte[]> ConvertToByteArrayAsync(string inputFile, string outputDir, FormatConversionType format, CancellationToken cancellation = default);

        /// <summary>
        /// Converte um arquivo para o formato especificado e retorna o conteúdo convertido como um stream em memória.
        /// </summary>
        /// <param name="inputFile">Caminho completo do arquivo de entrada.</param>
        /// <param name="outputDir">Diretório onde o arquivo convertido será salvo temporariamente.</param>
        /// <param name="format">Formato de conversão desejado.</param>
        /// <param name="logger">Logger utilizado para registrar informações e erros da conversão.</param>
        /// <param name="cancellation">Token opcional para cancelamento da operação.</param>
        /// <returns>Conteúdo convertido do arquivo como <see cref="MemoryStream"/>.</returns>
        Task<Stream> ConvertToMemoryStreamAsync(string inputFile, string outputDir, FormatConversionType format, CancellationToken cancellation = default);
    }
}
