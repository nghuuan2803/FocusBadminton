namespace Sh.Common
{
    public class PaginatedData<T>
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }

        public IEnumerable<T> Data { get; set; }

        public PaginatedData(IEnumerable<T> data, int pageIndex,
            int pageSize, int totalItem)
        {
            Data = data;
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalItems = totalItem;
            TotalPages = (int)Math.Ceiling((double)totalItem / pageSize);
        }

    }
}
