using CRM_Homestay.Contract.Bases;

namespace CRM_Homestay.Contract.Rooms
{
    public class RoomFilterDto : FilterBase, IPagedResultRequest
    {
        public Guid? BranchId { get; set; }
        public Guid? RoomTypeId { get; set; }
        public int? GuestCounts { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public List<Guid>? AmenityIds { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
