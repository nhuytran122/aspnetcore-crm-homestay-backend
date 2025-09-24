namespace CRM_Homestay.Contract.Bases;

public interface IPagedResultRequest
{
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
}