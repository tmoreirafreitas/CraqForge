using CraqForge.DocuCraft.Abstractions.FileManagement.Extractions.Pdf;

namespace CraqForge.DocuCraft.Abstractions.FileFactory
{
    /// <summary>
    /// Define um contrato para uma fábrica responsável por instanciar objetos <see cref="IPdfExtractor"/>
    /// a partir de arquivos PDF representados como um array de bytes.
    /// </summary>
    public interface IPdfPageContentAnalyzerFactory<T> where T : IPdfPageContent, new()
    {
        /// <summary>
        /// Cria uma instância de <see cref="IPdfExtractor"/> para manipulação e extração de dados do PDF informado.
        /// </summary>
        /// <param name="pdfBytes">Conteúdo binário do arquivo PDF.</param>
        /// <returns>Uma instância de <see cref="IPdfExtractor"/> associada ao documento fornecido.</returns>
        IPdfContentAnalyzer<T> Create(byte[] pdfBytes);
    }
}
