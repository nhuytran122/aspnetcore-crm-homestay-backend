using CRM_Homestay.Contract.Bases;
using CRM_Homestay.Core.Enums;

namespace CRM_Homestay.Contract.Branches
{
    public class BranchFilterDto : FilterBase
    {
        public BranchStatuses? Status { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
