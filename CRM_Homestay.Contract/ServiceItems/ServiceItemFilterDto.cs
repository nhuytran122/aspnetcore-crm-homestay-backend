using CRM_Homestay.Contract.Bases;
using CRM_Homestay.Core.Enums;

namespace CRM_Homestay.Contract.ServiceItems
{
    public class ServiceItemFilterDto : FilterBase, IPagedResultRequest
    {
        public ServiceItemTypes? Type { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
