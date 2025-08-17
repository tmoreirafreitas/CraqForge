namespace CraqForge.DocuCraft.Abstractions.FileManagement.Extractions.Pdf
{
    /// <summary>
    /// Interface para análise e extração de conteúdo de documentos PDF.
    /// Permite extração de texto, imagens e metadados, bem como verificações heurísticas sobre o conteúdo das páginas.
    /// </summary>
    /// <typeparam name="T">Tipo de conteúdo de página PDF, implementando <see cref="IPdfPageContent"/>.</typeparam>
    public interface IPdfContentAnalyzer<T> where T : IPdfPageContent, new()
    {
        /// <summary>
        /// Obtém o número total de páginas do PDF.
        /// </summary>
        /// <returns>Quantidade de páginas.</returns>
        int GetPageCount();

        /// <summary>
        /// Extrai todo o texto do PDF como uma única string.
        /// </summary>
        /// <param name="cancellationToken">Token para cancelamento da operação.</param>
        /// <returns>Texto completo do PDF.</returns>
        Task<string> ExtractFullTextAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Extrai o texto do PDF organizado por página.
        /// </summary>
        /// <param name="cancellationToken">Token para cancelamento da operação.</param>
        /// <returns>Lista de conteúdos por página.</returns>
        IReadOnlyList<T> ExtractTextByPage(CancellationToken cancellationToken = default);

        /// <summary>
        /// Extrai o texto apenas de páginas específicas do PDF.
        /// </summary>
        /// <param name="pageNumbers">Números das páginas a serem extraídas.</param>
        /// <returns>Conteúdo das páginas solicitadas.</returns>
        IReadOnlyList<T> ExtractSpecificPagesAsText(params int[] pageNumbers);

        /// <summary>
        /// Extrai o texto de uma página específica.
        /// </summary>
        /// <param name="pageNumber">Número da página (1-based).</param>
        /// <returns>Texto da página.</returns>
        string ExtractPageAsText(int pageNumber);

        /// <summary>
        /// Extrai todas as páginas do PDF como imagens, retornando cada página em memória.
        /// </summary>
        /// <param name="cancellationToken">Token para cancelamento da operação.</param>
        /// <returns>Lista de conteúdos de página com imagens em PNG.</returns>
        Task<IReadOnlyList<T>> ExtractAllPagesAsImagesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Extrai uma página específica do PDF como imagem em memória.
        /// </summary>
        /// <param name="pageNumber">Número da página (1-based).</param>
        /// <param name="cancellationToken">Token para cancelamento da operação.</param>
        /// <returns>Imagem da página em PNG como array de bytes.</returns>
        Task<byte[]> ExtractPageAsImageAsync(int pageNumber, CancellationToken cancellationToken = default);

        /// <summary>
        /// Extrai metadados do PDF, incluindo número de páginas, versão, autoria e datas de criação/modificação.
        /// </summary>
        /// <param name="fileName">Nome do arquivo PDF (opcional).</param>
        /// <returns>Objeto <see cref="PdfInfo"/> com metadados.</returns>
        PdfInfo ExtractInfo(string? fileName = "");

        /// <summary>
        /// Verifica se uma página contém apenas imagens, sem texto detectável.
        /// </summary>
        /// <param name="pageNumber">Número da página (1-based).</param>
        /// <returns>True se a página contém apenas imagens, false caso contrário.</returns>
        bool PageContainsOnlyImages(int pageNumber);

        /// <summary>
        /// Determina se o texto extraído de uma página provavelmente provém de um scanner.
        /// </summary>
        /// <param name="extractedText">Texto extraído.</param>
        /// <returns>True se o texto parece ter sido digitalizado, false caso contrário.</returns>
        bool IsTextLikelyFromScanner(string extractedText);

        /// <summary>
        /// Verifica se uma página necessita de OCR (caso não possua texto ou contenha apenas imagens/scans).
        /// </summary>
        /// <param name="pageNumber">Número da página (1-based).</param>
        /// <returns>True se a página precisa de OCR, false caso contrário.</returns>
        bool PageNeedsOcr(int pageNumber);
    }
}
