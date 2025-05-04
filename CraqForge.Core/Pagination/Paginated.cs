namespace CraqForge.Core.Pagination
{
    /// <summary>
    /// Representa um conjunto paginado de resultados com metadados úteis de paginação.
    /// </summary>
    /// <typeparam name="T">Tipo dos itens retornados.</typeparam>
    public class Paginated<T>
    {
        public IEnumerable<T> Items { get; init; } = [];
        public int Page { get; init; }
        public int PageSize { get; init; }
        public int TotalCount { get; init; }

        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPrevious => Page > 1;
        public bool HasNext => Page < TotalPages;

        public Paginated() { }

        public Paginated(IEnumerable<T> items, int totalCount, int page, int pageSize)
        {
            Items = items;
            TotalCount = totalCount;
            Page = page;
            PageSize = pageSize;
        }

        public static Paginated<T> Create(IEnumerable<T> source, int totalCount, int page, int pageSize) =>
            new(source, totalCount, page, pageSize);
    }
}

