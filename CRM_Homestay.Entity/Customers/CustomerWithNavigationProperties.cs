using CRM_Homestay.Entity.CustomerGroups;

namespace CRM_Homestay.Entity.Customers;

public class CustomerWithNavigationProperties
{
    public Customer? Customer { get; set; }
    public DateTime? NextDateVisit { get; set; }
    public CustomerGroup? Group { get; set; }
    public bool? IsGatePassSent { get; set; }
}