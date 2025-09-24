namespace CRM_Homestay.Contract.Bases;

public class PagedResponse<T>
{
    public List<T> Data { get; set; } = new();
    public PaginationMetaDto Meta { get; set; } = new();
}

public class PagedResponseCustomerMenu<T, TY>
{
    public List<T> CustomerMenus { get; set; } = new();
    public List<TY> Data { get; set; } = new();
    public PaginationMetaDto Meta { get; set; } = new();
}

public class PaginationMetaDto
{
    public int TotalItems { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}
