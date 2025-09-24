namespace CRM_Homestay.Contract.Customers;

public class CustomerAccountTokenDto
{
    public Guid? CustomerId { get; set; }
    public Guid CustomerAccountId { get; set; }
    public string Type { get; set; } = "";
    public DateTime ExpiresIn { get; set; }
}
