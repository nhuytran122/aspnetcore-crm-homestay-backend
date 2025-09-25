using CRM_Homestay.Contract.Bases;
using CRM_Homestay.Core.Enums;

namespace CRM_Homestay.Contract.Branches
{
    public class BranchFilterDto : FilterBase, IPagedResultRequest
    {
        public BranchStatuses? Status { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
