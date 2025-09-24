namespace CRM_Homestay.Contract.Bases
{
    public class PagingRequest<T>
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public Dictionary<string, string> Filters { get; set; } = new Dictionary<string, string>();
    }
}
