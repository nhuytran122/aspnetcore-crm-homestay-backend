using CRM_Homestay.Contract.Bases;

namespace CRM_Homestay.Contract.RoomTypes
{
    public class RoomTypeFilterDto : FilterBase, IPagedResultRequest
    {
        public int? GuestCounts { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
