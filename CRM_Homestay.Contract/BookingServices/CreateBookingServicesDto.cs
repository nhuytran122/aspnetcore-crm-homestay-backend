namespace CRM_Homestay.Contract.BookingServices
{
    public class CreateBookingServicesDto
    {
        public List<CreateBookingServiceDto> Services { get; set; } = new();
    }
    public class CreateBookingServiceDto
    {
        public Guid ServiceId { get; set; }
        public Guid? AssignedStaffId { get; set; }
        public Guid? BookingRoomId { get; set; }
        public double? Quantity { get; set; }
        public string? Description { get; set; }
        public List<CreateBookingServiceItemDto>? Items { get; set; }
    }

    public class CreateBookingServiceItemDto
    {
        public Guid ServiceItemId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

}