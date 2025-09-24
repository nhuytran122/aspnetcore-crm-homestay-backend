namespace CRM_Homestay.Entity.Bases;

public class PagedResult<T>
{
    public int TotalItems { get; set; }
    public int TotalPage { get; set; }
    public List<T> Items { get; set; } = new List<T>();
}