namespace CRM_Homestay.Contract.BookingServices
{
    public class UpdateBookingServiceDto
    {
        public int? Quantity { get; set; }
        public Guid? AssignedStaffId { get; set; }
        public string? Description { get; set; }
        public List<UpdateBookingServiceItemDto>? Items { get; set; }
    }

    public class UpdateBookingServiceItemDto
    {
        public Guid? Id { get; set; }  
        public Guid ServiceItemId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

}
