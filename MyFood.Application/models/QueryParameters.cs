namespace MyFood.Application.Models
{
    public class QueryParameters
    {
        private const int maxPageCount = 50;
        public int Page { get; set; } = 1;

        private int _pageCount = maxPageCount;
        public int PageCount
        {
            get { return _pageCount; }
            set { _pageCount = (value > maxPageCount) ? maxPageCount : value; }
        }

        public string? Query { get; set; } = "";
        public string OrderBy { get; set; } = "Name";

        public bool HasNext(int totalCount) => Page < GetTotalPages(totalCount);
        public bool HasPrevious() => Page > 1;
        public int GetTotalPages(int totalCount) => (int)Math.Ceiling(totalCount / (double)PageCount);
    }