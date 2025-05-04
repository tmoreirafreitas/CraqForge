namespace CraqForge.Core.FileManagement.Results
{
    /// <summary>
    /// Representa as informações extraídas de um documento do tipo DOCX.
    /// </summary>
    public class DocxInformationResult : DocumentInformationResult
    {
        /// <summary>
        /// Descrição adicional do documento, quando disponível.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Versão do aplicativo utilizado para criar ou modificar o documento.
        /// </summary>
        public string ApplicationVersion { get; set; } = string.Empty;
    }
}
