namespace CraqForge.Core.FileManagement.Results
{
    /// <summary>
    /// Representa as informações extraídas de um documento do tipo PDF.
    /// </summary>
    public class PdfInformationResult : DocumentInformationResult
    {
        /// <summary>
        /// Tamanho total do arquivo PDF em bytes.
        /// </summary>
        public long FileSizeInBytes { get; set; }

        /// <summary>
        /// Número total de páginas do PDF.
        /// </summary>
        public int NumberOfPages { get; set; }

        /// <summary>
        /// Versão do PDF (ex: 1.7).
        /// </summary>
        public string PdfVersion { get; set; } = string.Empty;

        /// <summary>
        /// Aplicativo usado originalmente para criar o PDF.
        /// </summary>
        public string Creator { get; set; } = string.Empty;

        /// <summary>
        /// Ferramenta ou biblioteca usada para produzir o PDF.
        /// </summary>
        public string Producer { get; set; } = string.Empty;

        /// <summary>
        /// Indica se o documento segue o padrão PDF/A (arquivamento).
        /// </summary>
        public bool IsPdfA { get; set; }

        /// <summary>
        /// Indica se o PDF possui metadados XMP incorporados.
        /// </summary>
        public bool HasXmpMetadata { get; set; }
    }
}
