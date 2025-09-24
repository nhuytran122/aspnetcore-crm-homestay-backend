namespace CRM_Homestay.Contract.Bases
{
    public class PageResult<T>
    {
        public int TotalRecord { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public List<T> Items { get; set; } = new List<T>();
    }
}
