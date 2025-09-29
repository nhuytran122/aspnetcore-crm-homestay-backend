using Microsoft.AspNetCore.Http;

namespace CRM_Homestay.Contract.Rooms
{
    public class CreateRoomDto
    {
        public int? RoomNumber { get; set; }
        public bool IsActive { get; set; }
        public Guid BranchId { get; set; }
        public Guid RoomTypeId { get; set; }
        public List<IFormFile>? Images { get; set; }
        public List<CreateAmenityForRoomDto>? RoomAmenities { get; set; }
    }

    public class CreateAmenityForRoomDto
    {
        public Guid AmenityId { get; set; }
        public int? Quantity { get; set; }
    }
}