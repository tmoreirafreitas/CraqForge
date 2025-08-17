namespace CraqForge.Core.Abstractions.FileManagement.Models
{
    public record DataTable
    {
        public string? Name { get; init; }
        public IList<DataRow> Rows { get; init; } = [];
    }

    public record DataRow
    {
        public IList<DataColumn> Columns { get; init; } = [];
    }

    public record DataColumn
    {
        public string? Name { get; init; }
        public string? Content { get; init; }
        public IList<DataTable> Tables { get; init; } = [];
    }
}
