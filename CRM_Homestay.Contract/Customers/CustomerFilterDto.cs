using CRM_Homestay.Contract.Bases;
using CRM_Homestay.Core.Enums;

namespace CRM_Homestay.Contract.Customers;

public class CustomerFilterDto : FilterBase, IPagedResultRequest
{
    public CustomerTypes? Type { get; set; }
    public Guid? GroupId { get; set; }
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public DateTime? LastVisitFrom { get; set; }
    public DateTime? LastVisitTo { get; set; }

    public DateTime? NextVisitFrom { get; set; }
    public DateTime? NextVisitTo { get; set; }
    public bool? IsGatePassSent { get; set; }
}