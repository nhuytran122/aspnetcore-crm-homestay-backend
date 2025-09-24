using CRM_Homestay.Contract.Bases;
using CRM_Homestay.Core.Enums;

namespace CRM_Homestay.Contract.Users;

public class UserFilterDto : FilterBase, IPagedResultRequest
{
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public Gender? Gender { get; set; }
    public Guid? RoleId { get; set; }
    public bool? IsActive { get; set; }
}