using CraqForge.Core.Abstractions.Identification;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace CraqForge.Core.Identification
{
    /// <summary>
    /// Implementação da fábrica de identificadores. Gera GUIDs determinísticos com base em conteúdo e chaves fornecidas.
    /// </summary>
    internal class IdentifierFactory : IIdentifierFactory
    {
        /// <summary>
        /// Cria um identificador único para um item filho, a partir de um identificador pai e um número.
        /// </summary>
        /// <param name="parentId">Identificador do item pai.</param>
        /// <param name="number">Número da página ou item filho.</param>
        /// <returns>Guid gerado para o item filho.</returns>
        public Guid CreateSubItemId(Guid parentId, int number)
        {
            var key = $"{parentId}_page_{number}";
            return GenerateDeterministicGuid(key);
        }

        /// <summary>
        /// Gera um identificador determinístico com base em uma chave fornecida.
        /// </summary>
        /// <param name="key">Chave para gerar o identificador.</param>
        /// <returns>Guid determinístico gerado.</returns>
        public Guid GenerateDeterministicGuid(string key)
        {
            var normalized = Normalize(key);

            var hash = SHA1.HashData(Encoding.UTF8.GetBytes(normalized));

            // Usa os primeiros 16 bytes do hash para criar o GUID
            var guidBytes = new byte[16];
            Array.Copy(hash, guidBytes, 16);

            return new Guid(guidBytes);
        }

        /// <summary>
        /// Gera um identificador determinístico a partir de uma chave e conteúdo fornecido.
        /// </summary>
        /// <param name="key">Chave para gerar o identificador.</param>
        /// <param name="content">Conteúdo em formato de bytes para gerar o identificador.</param>
        /// <returns>Guid determinístico gerado.</returns>
        public Guid GenerateDeterministicGuidFromKeyAndContent(string key, byte[] content)
        {
            var normalized = Normalize(key);
            var hash = ComputeSha256Hash(content);
            var newKey = $"{normalized}_{hash}";
            return GenerateDeterministicGuid(newKey);
        }

        /// <summary>
        /// Normaliza uma string para remover caracteres especiais e espaços, e converte para minúsculas.
        /// </summary>
        /// <param name="input">Texto a ser normalizado.</param>
        /// <returns>Texto normalizado.</returns>
        public string Normalize(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // Pega o nome completo, com extensão
            var name = Path.GetFileName(input);

            // Remove acentos
            name = name.Normalize(NormalizationForm.FormD);
            name = new string(name.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray());

            // Remove apenas os parênteses e colchetes, mantendo o conteúdo interno
            name = name.Replace("(", "")
                       .Replace(")", "")
                       .Replace("[", "")
                       .Replace("]", "");

            // Substitui qualquer caractere que não seja letra, número ou espaço por underscore
            name = Regex.Replace(name, @"[^\w\s]", "_");

            // Substitui espaços por underscore
            name = Regex.Replace(name, @"\s+", "_");

            // Normaliza múltiplos underscores
            name = Regex.Replace(name, @"_+", "_");

            // Remove underscores do início e do fim
            name = name.Trim('_');

            // Converte para minúsculas
            return name.ToLowerInvariant();
        }

        /// <summary>
        /// Calcula o hash SHA-256 de um conteúdo fornecido.
        /// </summary>
        /// <param name="data">Conteúdo em formato de bytes.</param>
        /// <returns>Representação hexadecimal do hash SHA-256.</returns>
        public static string ComputeSha256Hash(byte[] data)
        {
            var hashBytes = SHA256.HashData(data);
            return Convert.ToHexString(hashBytes).ToLowerInvariant();
        }
    }
}