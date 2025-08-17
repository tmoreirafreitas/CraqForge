namespace CraqForge.Core.Abstractions.FileManagement.Models
{
    public record DocumentMetadata
    {
        private readonly List<DocumentMetadata> _childs = [];       
        public IReadOnlyList<DocumentMetadata> Childs => _childs;
        public DocumentMetadata? Parent { get; protected set; }
        public MetadataType Type { get; init; }
        public string Content { get; init; } = string.Empty;
        public int? Index { get; set; }       
        
        public DocumentMetadata SetChild(DocumentMetadata child)
        {
            _childs.Add(child);
            return this;
        }

        public DocumentMetadata Build()
        {
            foreach (var child in Childs)
                child.Parent = this;

            return this;
        }
    }
}
