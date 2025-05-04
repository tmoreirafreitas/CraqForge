namespace CraqForge.Core.Abstractions.Identification
{
    /// <summary>
    /// Interface para a criação de identificadores exclusivos baseados em conteúdo.
    /// </summary>
    public interface IIdentifierFactory
    {
        /// <summary>
        /// Cria um identificador único para um item filho, a partir de um identificador pai e um número.
        /// </summary>
        /// <param name="parentId">Identificador do item pai.</param>
        /// <param name="number">Número da página ou item filho.</param>
        /// <returns>Guid gerado para o item filho.</returns>
        Guid CreateSubItemId(Guid parentId, int number);

        /// <summary>
        /// Gera um identificador determinístico com base em uma chave fornecida.
        /// </summary>
        /// <param name="key">Chave para gerar o identificador.</param>
        /// <returns>Guid determinístico gerado.</returns>
        Guid GenerateDeterministicGuid(string key);

        /// <summary>
        /// Gera um identificador determinístico a partir de uma chave e conteúdo fornecido.
        /// </summary>
        /// <param name="key">Chave para gerar o identificador.</param>
        /// <param name="content">Conteúdo em formato de bytes para gerar o identificador.</param>
        /// <returns>Guid determinístico gerado.</returns>
        Guid GenerateDeterministicGuidFromKeyAndContent(string key, byte[] content);

        /// <summary>
        /// Normaliza uma string para remover caracteres especiais e espaços, e converte para minúsculas.
        /// </summary>
        /// <param name="input">Texto a ser normalizado.</param>
        /// <returns>Texto normalizado.</returns>
        string Normalize(string input);
    }
}
