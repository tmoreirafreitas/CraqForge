namespace CraqForge.DocuCraft.Abstractions.FileManagement
{
    /// <summary>
    /// Resolve o caminho do executável do LibreOffice (soffice) de acordo com o sistema operacional.
    /// Também provê fallback via Docker caso o LibreOffice não esteja instalado localmente.
    /// </summary>
    public interface ILibreOfficePathResolver
    {
        /// <summary>
        /// Obtém o caminho do executável do LibreOffice (soffice) conforme o sistema operacional.
        /// </summary>
        /// <returns>O caminho absoluto do executável do LibreOffice.</returns>
        /// <exception cref="FileNotFoundException">Lançado se o LibreOffice não for encontrado.</exception>
        string GetSofficePath();

        /// <summary>
        /// Verifica se o LibreOffice está instalado localmente.
        /// </summary>
        /// <returns>True se o LibreOffice foi encontrado no sistema; caso contrário, false.</returns>
        bool IsLibreOfficeInstalled();
        bool ShouldUseDocker { get; }

        /// <summary>
        /// Gera o comando Docker para executar a conversão do LibreOffice usando um contêiner.
        /// </summary>
        /// <param name="inputFile">Caminho do arquivo de entrada.</param>
        /// <param name="outputDir">Diretório onde o resultado será salvo.</param>
        /// <param name="formatDescription">Descrição do formato de conversão (ex: pdf, docx).</param>
        /// <returns>Comando completo para execução do LibreOffice via Docker.</returns>
        string GetDockerCommand(string inputFile, string outputDir, string formatDescription);
    }
}
