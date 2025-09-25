namespace CRM_Homestay.Contract.Bases;

public class PagedResultDto<T>
{
    public int TotalItems { get; set; }
    public int TotalPage { get; set; }
    public List<T> Items { get; set; } = new List<T>();
    public int? TotalQuantity { get; set; }
    public int? TotalQuantityPaid { get; set; }
    public int? TotalQuantityDebt { get; set; }
    public decimal? TotalCashAdvance { get; set; }
    public decimal? TotalCashOfOrder { get; set; }
    public decimal? TotalSpentDuringDay { get; set; }
    public decimal? TotalAmountHeld { get; set; }
    public decimal? TotalOrderAmount { get; set; }
}