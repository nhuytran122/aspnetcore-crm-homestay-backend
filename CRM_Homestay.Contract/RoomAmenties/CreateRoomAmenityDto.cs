namespace CRM_Homestay.Contract.RoomAmenities
{
    public class CreateRoomAmenityDto
    {
        public Guid? RoomId { get; set; }
        public Guid AmenityId { get; set; }
        public int? Quantity;
    }
}