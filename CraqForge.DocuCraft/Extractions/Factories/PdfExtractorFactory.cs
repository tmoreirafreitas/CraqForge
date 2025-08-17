using CraqForge.DocuCraft.Abstractions;
using CraqForge.DocuCraft.Extractions.Pdf;

namespace CraqForge.DocuCraft.Extractions.Factories
{
    /// <summary>
    /// Factory helper para criar instâncias de <see cref="IPdfExtractor"/>.
    /// </summary>
    public static class PdfExtractorFactory
    {
        /// <summary>
        /// Cria um <see cref="PdfExtractor"/> a partir do conteúdo do PDF em memória.
        /// </summary>
        /// <param name="pdfContent">Conteúdo do PDF como array de bytes.</param>
        /// <returns>Instância de <see cref="IPdfExtractor"/> pronta para uso.</returns>
        /// <exception cref="ArgumentException">Se o conteúdo for nulo ou vazio.</exception>
        public static IPdfExtractor Create(byte[] pdfContent)
        {
            ValidatePdfContent(pdfContent);
            return new PdfExtractor(pdfContent);
        }

        /// <summary>
        /// Valida se o conteúdo do PDF é válido (não nulo e não vazio).
        /// </summary>
        /// <param name="pdfContent">Conteúdo do PDF.</param>
        /// <exception cref="ArgumentException">Se inválido.</exception>
        private static void ValidatePdfContent(byte[] pdfContent)
        {
            if (pdfContent == null || pdfContent.Length == 0)
                throw new ArgumentException("O PDF não pode ser nulo ou vazio.", nameof(pdfContent));
        }
    }
}
