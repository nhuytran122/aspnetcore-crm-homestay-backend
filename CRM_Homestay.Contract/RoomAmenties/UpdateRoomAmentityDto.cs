namespace CRM_Homestay.Contract.RoomAmenities
{
    public class UpdateRoomAmenityDto
    {
        public int? Quantity { get; set; }
        public Guid RoomId { get; set; }
        public Guid AmenityId { get; set; }
    }
}