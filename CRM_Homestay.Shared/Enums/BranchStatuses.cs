using System.ComponentModel;

namespace CRM_Homestay.Core.Enums
{
    public enum BranchStatuses
    {
        [Description("Không hoạt động")]
        Inactive = 0,
        [Description("Đang hoạt động")]
        Active = 1
    }
}
