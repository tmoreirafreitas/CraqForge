using CraqForge.Core.Identification;
using System.Text;

namespace CraqForge.Core.Tests
{
    public class IdentifierFactoryTests
    {
        private readonly IdentifierFactory _identifierFactory = new();

        [Fact]
        public void CreateSubItemId_ShouldGenerateDeterministicId()
        {
            var parentId = Guid.NewGuid();
            int number = 1;

            var subItemId = _identifierFactory.CreateSubItemId(parentId, number);

            Assert.NotEqual(Guid.Empty, subItemId);
            Assert.Equal(subItemId, _identifierFactory.CreateSubItemId(parentId, number));  // Deve ser determinístico
        }

        [Theory]
        [InlineData("sampleKey", "40dc3d9e-8583-056e-8cf7-ece035aab777")]
        [InlineData("anotherKey", "7a419852-dee3-8184-fb3e-62a92c7620ed")]
        public void GenerateDeterministicGuid_ShouldReturnConsistentGuid(string key, string expectedGuid)
        {
            var guid = _identifierFactory.GenerateDeterministicGuid(key);

            Assert.Equal(new Guid(expectedGuid), guid);  // A comparação é feita pelo valor do GUID gerado
        }

        [Fact]
        public void GenerateDeterministicGuidFromKeyAndContent_ShouldReturnConsistentGuid()
        {
            var key = "sampleKey";
            var content = Encoding.UTF8.GetBytes("some content");

            var guid = _identifierFactory.GenerateDeterministicGuidFromKeyAndContent(key, content);

            Assert.NotEqual(Guid.Empty, guid);
        }

        [Theory]
        [InlineData("Test File (1).txt", "test_file_1_txt")]
        [InlineData("Special @Name 123.doc", "special_name_123_doc")]
        [InlineData("File-With-Special#Characters.txt", "file_with_special_characters_txt")]
        public void Normalize_ShouldReturnNormalizedString(string input, string expected)
        {
            var normalized = _identifierFactory.Normalize(input);
            Assert.Equal(expected, normalized);
        }

        [Fact]
        public void Normalize_ShouldHandleEmptyString()
        {
            var normalized = _identifierFactory.Normalize("");
            Assert.Equal("", normalized);  // Deve retornar uma string vazia se a entrada for vazia
        }

        [Fact]
        public void ComputeSha256Hash_ShouldReturnCorrectHash()
        {
            var data = Encoding.UTF8.GetBytes("test data");

            var hash = IdentifierFactory.ComputeSha256Hash(data);

            Assert.Equal("916f0027a575074ce72a331777c3478d6513f786a591bd892da1a577bf2335f9", hash);
        }
    }
}