using CRM_Homestay.Contract.Bases;
using CRM_Homestay.Core.Enums;

namespace CRM_Homestay.Contract.Amenities
{
    public class AmenityFilterDto : FilterBase, IPagedResultRequest
    {
        public AmenityTypes? Type { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
