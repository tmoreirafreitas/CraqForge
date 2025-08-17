namespace CraqForge.DocuCraft.Abstractions.FileManagement.Conversions
{
    /// <summary>
    /// Define um contrato para conversão de conteúdo HTML em documentos PDF.
    /// </summary>
    public interface IHtmlToPdfConverter
    {
        /// <summary>
        /// Converte uma string contendo HTML diretamente em um arquivo PDF, retornado como array de bytes.
        /// </summary>
        /// <param name="htmlContent">Conteúdo HTML como texto.</param>
        /// <param name="cancellation">Token opcional para cancelamento da operação.</param>
        /// <returns>Conteúdo do PDF gerado como array de bytes.</returns>
        Task<byte[]> ConvertTextHtmlToPdfAsync(string htmlContent, CancellationToken cancellation = default);

        /// <summary>
        /// Converte um arquivo HTML (representado como um array de bytes) em um arquivo PDF, retornado como array de bytes.
        /// </summary>
        /// <param name="fileBytes">Conteúdo HTML em formato binário.</param>
        /// <param name="cancellation">Token opcional para cancelamento da operação.</param>
        /// <returns>Conteúdo do PDF gerado como array de bytes.</returns>
        Task<byte[]> ConvertTextHtmlToPdfAsync(byte[] fileBytes, CancellationToken cancellation = default);
    }
}
