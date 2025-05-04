namespace CraqForge.Core.Abstractions.FileManagement
{
    /// <summary>
    /// Interface responsável pela conversão e identificação de formatos de arquivos,
    /// incluindo a obtenção de tipos MIME, extensões e tipos de formato baseados em nomes de arquivos.
    /// </summary>
    public interface IFormatConversion
    {
        /// <summary>
        /// Obtém o tipo de conteúdo (MIME type) correspondente ao formato fornecido.
        /// </summary>
        /// <param name="format">Tipo de formato do arquivo.</param>
        /// <returns>O tipo de conteúdo correspondente, ou "application/octet-stream" se não for encontrado.</returns>
        string GetContentType(FormatConversionType format);

        /// <summary>
        /// Obtém a extensão do arquivo correspondente ao tipo de formato fornecido.
        /// </summary>
        /// <param name="format">Tipo de formato.</param>
        /// <returns>A extensão do arquivo (com ponto), ou null se não for encontrada.</returns>
        string GetExtensionType(FormatConversionType format);

        /// <summary>
        /// Determina o tipo de formato com base na extensão do nome do arquivo.
        /// </summary>
        /// <param name="fileName">Nome do arquivo com extensão.</param>
        /// <returns>Tipo de formato correspondente, ou Undefined se não for reconhecido.</returns>
        FormatConversionType GetFormatType(string fileName);
    }
}
