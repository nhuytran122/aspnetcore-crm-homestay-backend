using CRM_Homestay.Contract.Bases;
using CRM_Homestay.Core.Enums;

namespace CRM_Homestay.Contract.Bookings
{
    public class BookingFilterDto : FilterBase, IPagedResultRequest
    {
        public Guid? BranchId { get; set; }
        public Guid? RoomTypeId { get; set; }
        public BookingStatuses? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

}