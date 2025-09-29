using CRM_Homestay.Contract.Bases;

namespace CRM_Homestay.Contract.HomestayServices
{
    public class HomestayServiceFilterDto : FilterBase, IPagedResultRequest
    {
        public bool? IsPrepaid { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
