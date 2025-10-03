using CRM_Homestay.Contract.Bases;
using CRM_Homestay.Core.Enums;

namespace CRM_Homestay.Contract.BookingServices
{
    public class BookingServiceDto : BaseEntityDto
    {
        public double? Quantity { get; set; }
        public string? Description { get; set; }
        public Guid BookingId { get; set; }
        public Guid ServiceId { get; set; }
        public string? ServiceName { get; set; }         
        public Guid? AssignedStaffId { get; set; }
        public string? AssignedStaffName { get; set; }   
        public decimal? TotalPrice { get; set; }
        public BookingServiceStatuses Status { get; set; }          
        public List<BookingServiceItemDto>? Items { get; set; }
    }
    public class BookingServiceItemDto
    {
        public Guid Id { get; set; }
        public Guid ServiceItemId { get; set; }
        public string? Identifier { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}