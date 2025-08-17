namespace CraqForge.Core.Abstractions.FileManagement
{
    /// <summary>
    /// Define operações de manipulação de arquivos no sistema de arquivos.
    /// Fornece métodos para download, criação de diretórios, geração de nomes temporários e persistência de arquivos.
    /// </summary>
    public interface IFileManagementSystem
    {
        /// <summary>
        /// Lê o conteúdo de um arquivo existente a partir do caminho informado.
        /// O conteúdo é retornado como um array de bytes.
        /// </summary>
        /// <param name="filePathName">Caminho completo do arquivo a ser lido.</param>
        /// <param name="cancellationToken">Token opcional para cancelamento da operação.</param>
        /// <returns>O conteúdo do arquivo como array de bytes.</returns>
        /// <exception cref="ArgumentException">Lançado quando o caminho fornecido é uma string vazia ou contém apenas espaços em branco.</exception>
        /// <exception cref="ArgumentNullException">Lançado quando o caminho do arquivo é nulo.</exception>
        /// <exception cref="FileNotFoundException">Lançado quando o arquivo não é encontrado no caminho informado.</exception>
        /// <exception cref="Exception">Lançado quando ocorre um erro inesperado durante a leitura do arquivo.</exception>
        Task<byte[]> DownloadAsync(string filePathName, CancellationToken cancellationToken = default);

        /// <summary>
        /// Cria um diretório no sistema de arquivos, caso ele ainda não exista.
        /// </summary>
        /// <param name="folderName">Caminho completo do diretório a ser criado.</param>
        /// <returns>O caminho completo do diretório existente ou recém-criado.</returns>
        /// <exception cref="ArgumentException">Lançado quando o nome do diretório é inválido ou vazio.</exception>
        /// <exception cref="Exception">Lançado quando ocorre um erro inesperado ao criar o diretório.</exception>
        string CreateFolder(string folderName);

        /// <summary>
        /// Gera um nome único para um arquivo temporário dentro de um diretório especificado.
        /// O nome do arquivo é baseado em um GUID.
        /// </summary>
        /// <param name="tempPath">Caminho do diretório temporário onde o arquivo será gerado.</param>
        /// <returns>O caminho completo do arquivo temporário gerado.</returns>
        /// <exception cref="ArgumentException">Lançado quando o caminho do diretório é inválido ou vazio.</exception>
        /// <exception cref="Exception">Lançado quando ocorre um erro inesperado ao gerar o nome do arquivo.</exception>
        string NewTempFileName(string tempPath);

        /// <summary>
        /// Persiste um arquivo no sistema de arquivos a partir de um array de bytes.
        /// Se o diretório do caminho fornecido não existir, ele será criado automaticamente.
        /// </summary>
        /// <param name="fileBytes">Conteúdo do arquivo em formato de array de bytes.</param>
        /// <param name="filePath">Caminho completo onde o arquivo será salvo.</param>
        /// <param name="cancellationToken">Token opcional para cancelamento da operação.</param>
        /// <exception cref="ArgumentNullException">Lançado quando o array de bytes é nulo.</exception>
        /// <exception cref="ArgumentException">Lançado quando o array de bytes está vazio.</exception>
        /// <exception cref="Exception">Lançado quando ocorre um erro inesperado ao salvar o arquivo.</exception>
        Task SaveFileAsync(byte[] fileBytes, string filePath, CancellationToken cancellationToken = default);
    }
}
