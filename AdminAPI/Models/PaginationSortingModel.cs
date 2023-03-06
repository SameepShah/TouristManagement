namespace AdminAPI.Models
{
    public class PaginationSortingModel
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string? SortColumn { get; set; }
        public bool SortOrder { get; set; }
    }
}
