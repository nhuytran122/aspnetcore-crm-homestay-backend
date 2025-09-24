using CRM_Homestay.Contract.CustomerGroups;

namespace CRM_Homestay.Contract.Customers;

public class CustomerWithNavigationPropertiesDto
{
    public CustomerDto Customer { get; set; } = new CustomerDto();
    public CustomerGroupDto Group { get; set; } = new CustomerGroupDto();
    public bool? IsDeletable { get; set; }
    public DateTime? NextDateVisit { get; set; }
    public bool? IsGatePassSent { get; set; }
}

public class CustomerBasicDto
{
    public string? GroupName { get; set; }
    public string? CustomerFullName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? AddressJoinedName { get; set; }
    public string? Locate { get; set; }
}