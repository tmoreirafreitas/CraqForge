namespace CraqForge.DocuCraft.Abstractions.FileManagement.Extractions.Pdf
{
    /// <summary>
    /// Representa o conteúdo de texto extraído de uma página de um arquivo PDF.
    /// Estruturas mais complexas podem implementar essa interface conforme necessário.
    /// </summary>
    public interface IPdfPageContent
    {
        /// <summary>
        /// Número da página no documento PDF.
        /// </summary>
        int Page { get; init; }

        /// <summary>
        /// Conteúdo de texto extraído da página.
        /// </summary>
        string? Text { get; init; }
    }
}
